using Assets.Scripts.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static readonly int InventorySize = 20;
    public static readonly int HotbarSize = 10;

    public BuildingController BuildingController { get; private set; }

    public ScriptableEntityData diamond;
    public ScriptableEntityData stick;
    public List<EntityBase> inventoryEntities;
    private PhotonView photonView;
    private EquipmentManager equipmentManager;

    public delegate void OnEntityChanged();
    public OnEntityChanged OnEntityChangedCallback;

    private void Awake()
    {
        BuildingController = FindObjectOfType<BuildingController>();
    }

    private void Start()
    {
        inventoryEntities = new List<EntityBase>(new EntityBase[InventorySize + HotbarSize]);
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!photonView.isMine)
            return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            AddEntityById(stick.Id, 10);
            AddEntityById(diamond.Id, 10);
        }
#endif
    }

    private void AddNewEntityStackById(int entityId, int stackSize)
    {
        //If inventory is full we drop the entities on the floor
        if (!inventoryEntities.FirstNullIndexAt().HasValue)
        {
            EntityFactory.CreateWorldObject(transform.position, entityId, stackSize);
            return;
        }

        EntityBase entity = EntityFactory.CreateNewEntity(entityId, stackSize);
        var emptyIndex = inventoryEntities.FirstNullIndexAt();

        inventoryEntities[emptyIndex.Value] = entity;
        OnEntityChangedCallback?.Invoke();
    }

    private void FillEntityStacksById(int entityId, int stackSize)
    {
        EntityBase entity = EntityFactory.CreateNewEntity(entityId, stackSize);

        //Check if the entity to add is a Resource entity.
        if (entity.GetType() == typeof(Resource))
        {
            int entitiesToAdd = entity.StackSize;

            //Get all the entities in the inventory where the id is the same as the entity to add id.
            var existingEntities = inventoryEntities.Where(x => x?.Id == entity.Id && entity.StackSize < EntityBase.MAXSTACKSIZE).ToArray();

            //There are uncompleted stacks, we can add entities to them.
            if (existingEntities != null)
            {
                //Loop through all the existing entity stacks and check if there is any room.
                for (int i = 0; i < existingEntities.Length; i++)
                {
                    //We should be done adding entities, return
                    if (entitiesToAdd == 0)
                        return;

                    Resource currentStack = existingEntities[i] as Resource;
                    int availableAmount = EntityBase.MAXSTACKSIZE - currentStack.StackSize;
                    if (availableAmount >= entitiesToAdd)
                    {
                        currentStack.StackSize += entitiesToAdd;
                        entitiesToAdd = 0;
                        OnEntityChangedCallback?.Invoke();
                    }
                    else
                    {
                        currentStack.StackSize = EntityBase.MAXSTACKSIZE;
                        entitiesToAdd -= availableAmount;
                        OnEntityChangedCallback?.Invoke();
                    }
                }
                if (entitiesToAdd > 0)
                    AddNewEntityStackById(entityId, entitiesToAdd);
            }
            else
            {
                AddNewEntityStackById(entityId, entitiesToAdd);
            }
        }
        else
        {
            AddNewEntityStackById(entityId, stackSize);
        }
    }

    /// <summary>
    /// Removes an entity from the inventory
    /// </summary>
    public void RemoveEntityAtIndex(int index)
    {
        if (index < inventoryEntities.Count && inventoryEntities[index] != null)
        {
            inventoryEntities[index] = null;
            OnEntityChangedCallback?.Invoke();
        }
        else
            print($"Tried removing an entity at index {index} but it couldnt be found in the inventory");
    }

    public void SwapEntities(int indexA, int indexB)
    {
        inventoryEntities.Swap(indexA, indexB);
        OnEntityChangedCallback?.Invoke();
    }

    public int GetEntityAmountById(int entityId)
    {
        if (IsInventoryEmpty())
            return 0;

        int temp = 0;
        for (int i = 0; i < inventoryEntities.Count; i++)
        {
            if (inventoryEntities[i]?.Id == entityId)
            {
                if (inventoryEntities[i].GetType() == typeof(Resource))
                    temp += inventoryEntities[i].StackSize;
                else temp += 1;
            }
        }
        return temp;
    }
    private bool IsInventoryEmpty()
    {
        return inventoryEntities == null;
    }

    public bool CheckAmountById(int entityId, int amountNeeded)
    {
        return (GetEntityAmountById(entityId) >= amountNeeded);
    }

    /// <summary>
    /// Removes entities based on the entity id and the amount of entities to remove. starting at the back of the inventory
    /// THE ENTITIES GET DESTROYED!!
    /// </summary>
    /// <param name="entityId">The id of the entity to remove</param>
    /// <param name="amountToRemove">The amount of entities to remove</param>
    public void RemoveEntityByIdBackwards(int entityId, int amountToRemove = 1)
    {
        if (!CheckAmountById(entityId, amountToRemove))
        {
            Debug.LogError($"Inventory -- Trying to remove {amountToRemove} x entity {entityId}, but we dont have that many");
            return;
        }

        //Remove entities from inventory, start at the back of the inventory.
        //TODO: Only check the entities with the required ID have to refactored removeEntities and other things aswell
        for (int i = inventoryEntities.Count - 1; i >= 0; i--)
        {
            //If all the entities are removed we can stop
            if (amountToRemove == 0)
                return;

            if (inventoryEntities[i]?.Id == entityId)
            {
                //Check if the entity is a resource if so, we can take entity of the stacksize.
                if (inventoryEntities[i].GetType() == typeof(Resource))
                {
                    Resource currentStack = (Resource)inventoryEntities[i];
                    if (amountToRemove >= currentStack.StackSize)
                    {
                        amountToRemove -= currentStack.StackSize;
                        RemoveEntityAtIndex(i);
                    }
                    else
                    {
                        currentStack.StackSize -= amountToRemove;
                        amountToRemove = 0;
                        OnEntityChangedCallback?.Invoke();
                        return;
                    }
                }
                //If it aint a resource we just get the single entity.
                else
                {
                    amountToRemove--;
                    RemoveEntityAtIndex(i);
                    OnEntityChangedCallback?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Removes entities based on the entity id and the amount of entities to remove. starting at the start of the inventory
    /// THE ENTITIES GET DESTROYED!!
    /// </summary>
    /// <param name="entityId">The id of the entity to remove</param>
    /// <param name="amountToRemove">The amount of entities to remove</param>
    public void RemoveEntityById(int entityId, int amountToRemove = 1)
    {
        if (!CheckAmountById(entityId, amountToRemove))
        {
            Debug.LogError($"Inventory -- Trying to remove {amountToRemove} x entity {entityId}, but we dont have that many");
            return;
        }

        //Remove entities from inventory, start at the back of the inventory.
        //TODO: Only check the entities with the required ID have to refactored removeEntities and other things aswell
        for (int i = 0; i < inventoryEntities.Count; i++)
        {
            //If all the entities are removed we can stop
            if (amountToRemove == 0)
                return;

            if (inventoryEntities[i]?.Id == entityId)
            {
                //Check if the entity is a resource if so, we can take entities of the stacksize.
                if (inventoryEntities[i].GetType() == typeof(Resource))
                {
                    Resource currentStack = (Resource)inventoryEntities[i];
                    if (amountToRemove >= currentStack.StackSize)
                    {
                        amountToRemove -= currentStack.StackSize;
                        RemoveEntityAtIndex(i);
                    }
                    else
                    {
                        currentStack.StackSize -= amountToRemove;
                        amountToRemove = 0;
                        OnEntityChangedCallback?.Invoke();
                        return;
                    }
                }
                //If it aint a resource we just get the single entity.
                else
                {
                    amountToRemove--;
                    RemoveEntityAtIndex(i);
                    OnEntityChangedCallback?.Invoke();
                }
            }
        }
    }

    public void AddEntityById(int entityId, int stackSize = 1)
    {
        if (!photonView.isMine)
            return;

        EntityBase entity = EntityFactory.CreateNewEntity(entityId, stackSize);

        if (!inventoryEntities.FirstNullIndexAt().HasValue)
        {
            //Check if we are adding a resource entity, if so we check if we have full stacks of the entity.
            if (entity.GetType() == typeof(Resource))
                if (GetEntityAmountById(entity.Id) % 64 != 0)
                    FillEntityStacksById(entityId, stackSize);

                else
                    EntityFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, entity.Id, stackSize);
        }
        else
        {
            if (entity.GetType() == typeof(Resource))
            {
                FillEntityStacksById(entityId, stackSize);
            }
            else
            {
                AddNewEntityStackById(entityId, stackSize);
            }
        }
    }

    public void AddEntityAtIndex(int entityId, int index, int stackSize = 1)
    {
        if(index < 0 || inventoryEntities[index] != null)
        {
            Debug.LogError("Inventory -- AddEntityAtIndex -- invalid index");
            AddEntityById(entityId, stackSize);
        }
        else
        {
            EntityBase entity = EntityFactory.CreateNewEntity(entityId, stackSize);
            inventoryEntities[index] = entity;
            OnEntityChangedCallback?.Invoke();
        }
    }

    /// <summary>
    /// Removes the required entities for the craftingRecipe
    /// </summary>
    /// <param name="recipe">The recipe to craft</param>
    /// <returns>Whether there are enough materials to craft this recipe</returns>
    public bool RemoveEntitiesForCrafting(CraftingRecipe recipe)
    {
        for (int i = 0; i < recipe.requiredEntities.Count; i++)
        {
            var requiredEntities = recipe.requiredEntities[i];
            if (!CheckAmountById(requiredEntities.entity.Id, requiredEntities.amount * recipe.amountToCraft))
            {
                Debug.Log($"Not enough {requiredEntities.entity.name} to craft {recipe.result.entity.name}");
                return false;
            }
        }

        for (int i = 0; i < recipe.requiredEntities.Count; i++)
        {
            var requiredEntities = recipe.requiredEntities[i];
            RemoveEntityById(requiredEntities.entity.Id, requiredEntities.amount * recipe.amountToCraft);
        }

        return true;
    }

    public int GetMaxCrafts(CraftingRecipe recipe)
    {
        int maxCrafts = int.MaxValue;
        foreach (var craftingEntity in recipe.requiredEntities)
        {
            int temp = GetEntityAmountById(craftingEntity.entity.Id) / craftingEntity.amount;
            if (temp < maxCrafts)
                maxCrafts = temp;
        }
        return maxCrafts;
    }
}
