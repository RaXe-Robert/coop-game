using Assets.Scripts.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static readonly int InventorySize = 20;
    public static readonly int HotbarSize = 10;

    public BuildingController BuildingController { get; private set; }

    public ScriptableItemData diamond;
    public ScriptableItemData stick;
    public List<Item> inventoryEntities;
    private PhotonView photonView;
    private EquipmentManager equipmentManager;

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    private void Awake()
    {
        BuildingController = FindObjectOfType<BuildingController>();
    }

    private void Start()
    {
        inventoryEntities = new List<Item>(new Item[InventorySize + HotbarSize]);
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!photonView.isMine)
            return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            AddItemById(stick.Id, 10);
            AddItemById(diamond.Id, 10);
        }
#endif
    }

    private void AddNewItemStackById(int itemId, int stackSize)
    {
        //If inventory is full we drop the entities on the floor
        if (!inventoryEntities.FirstNullIndexAt().HasValue)
        {
            ItemFactory.CreateWorldObject(transform.position, itemId, stackSize);
            return;
        }

        Item item= ItemFactory.CreateNewItem(itemId, stackSize);
        var emptyIndex = inventoryEntities.FirstNullIndexAt();

        inventoryEntities[emptyIndex.Value] = item;
        OnItemChangedCallback?.Invoke();
    }

    private void FillItemStacksById(int itemId, int stackSize)
    {
        Item item = ItemFactory.CreateNewItem(itemId, stackSize);

        //Check if the item to add is a Resource item.
        if (item.GetType() == typeof(Resource))
        {
            int entitiesToAdd = item.StackSize;

            //Get all the entities in the inventory where the id is the same as the item to add id.
            var existingEntities = inventoryEntities.Where(x => x?.Id == item.Id && item.StackSize < Item.MAXSTACKSIZE).ToArray();

            //There are uncompleted stacks, we can add entities to them.
            if (existingEntities != null)
            {
                //Loop through all the existing item stacks and check if there is any room.
                for (int i = 0; i < existingEntities.Length; i++)
                {
                    //We should be done adding entities, return
                    if (entitiesToAdd == 0)
                        return;

                    Resource currentStack = existingEntities[i] as Resource;
                    int availableAmount = Item.MAXSTACKSIZE - currentStack.StackSize;
                    if (availableAmount >= entitiesToAdd)
                    {
                        currentStack.StackSize += entitiesToAdd;
                        entitiesToAdd = 0;
                        OnItemChangedCallback?.Invoke();
                    }
                    else
                    {
                        currentStack.StackSize = Item.MAXSTACKSIZE;
                        entitiesToAdd -= availableAmount;
                        OnItemChangedCallback?.Invoke();
                    }
                }
                if (entitiesToAdd > 0)
                    AddNewItemStackById(itemId, entitiesToAdd);
            }
            else
            {
                AddNewItemStackById(itemId, entitiesToAdd);
            }
        }
        else
        {
            AddNewItemStackById(itemId, stackSize);
        }
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    public void RemoveItemAtIndex(int index)
    {
        if (index < inventoryEntities.Count && inventoryEntities[index] != null)
        {
            inventoryEntities[index] = null;
            OnItemChangedCallback?.Invoke();
        }
        else
            print($"Tried removing an item at index {index} but it couldnt be found in the inventory");
    }

    public void SwapEntities(int indexA, int indexB)
    {
        inventoryEntities.Swap(indexA, indexB);
        OnItemChangedCallback?.Invoke();
    }

    public int GetItemAmountById(int itemId)
    {
        if (IsInventoryEmpty())
            return 0;

        int temp = 0;
        for (int i = 0; i < inventoryEntities.Count; i++)
        {
            if (inventoryEntities[i]?.Id == itemId)
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

    public bool CheckAmountById(int itemId, int amountNeeded)
    {
        return (GetItemAmountById(itemId) >= amountNeeded);
    }

    /// <summary>
    /// Removes entities based on the item id and the amount of entities to remove. starting at the back of the inventory
    /// THE ENTITIES GET DESTROYED!!
    /// </summary>
    /// <param name="itemId">The id of the item to remove</param>
    /// <param name="amountToRemove">The amount of entities to remove</param>
    public void RemoveItemByIdBackwards(int itemId, int amountToRemove = 1)
    {
        if (!CheckAmountById(itemId, amountToRemove))
        {
            Debug.LogError($"Inventory -- Trying to remove {amountToRemove} x item {itemId}, but we dont have that many");
            return;
        }

        //Remove entities from inventory, start at the back of the inventory.
        //TODO: Only check the entities with the required ID have to refactored removeEntities and other things aswell
        for (int i = inventoryEntities.Count - 1; i >= 0; i--)
        {
            //If all the entities are removed we can stop
            if (amountToRemove == 0)
                return;

            if (inventoryEntities[i]?.Id == itemId)
            {
                //Check if the item is a resource if so, we can take item of the stacksize.
                if (inventoryEntities[i].GetType() == typeof(Resource))
                {
                    Resource currentStack = (Resource)inventoryEntities[i];
                    if (amountToRemove >= currentStack.StackSize)
                    {
                        amountToRemove -= currentStack.StackSize;
                        RemoveItemAtIndex(i);
                    }
                    else
                    {
                        currentStack.StackSize -= amountToRemove;
                        amountToRemove = 0;
                        OnItemChangedCallback?.Invoke();
                        return;
                    }
                }
                //If it aint a resource we just get the single item.
                else
                {
                    amountToRemove--;
                    RemoveItemAtIndex(i);
                    OnItemChangedCallback?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Removes entities based on the item id and the amount of entities to remove. starting at the start of the inventory
    /// THE ENTITIES GET DESTROYED!!
    /// </summary>
    /// <param name="itemId">The id of the item to remove</param>
    /// <param name="amountToRemove">The amount of entities to remove</param>
    public void RemoveItemById(int itemId, int amountToRemove = 1)
    {
        if (!CheckAmountById(itemId, amountToRemove))
        {
            Debug.LogError($"Inventory -- Trying to remove {amountToRemove} x item {itemId}, but we dont have that many");
            return;
        }

        //Remove entities from inventory, start at the back of the inventory.
        //TODO: Only check the entities with the required ID have to refactored removeEntities and other things aswell
        for (int i = 0; i < inventoryEntities.Count; i++)
        {
            //If all the entities are removed we can stop
            if (amountToRemove == 0)
                return;

            if (inventoryEntities[i]?.Id == itemId)
            {
                //Check if the item is a resource if so, we can take entities of the stacksize.
                if (inventoryEntities[i].GetType() == typeof(Resource))
                {
                    Resource currentStack = (Resource)inventoryEntities[i];
                    if (amountToRemove >= currentStack.StackSize)
                    {
                        amountToRemove -= currentStack.StackSize;
                        RemoveItemAtIndex(i);
                    }
                    else
                    {
                        currentStack.StackSize -= amountToRemove;
                        amountToRemove = 0;
                        OnItemChangedCallback?.Invoke();
                        return;
                    }
                }
                //If it aint a resource we just get the single item.
                else
                {
                    amountToRemove--;
                    RemoveItemAtIndex(i);
                    OnItemChangedCallback?.Invoke();
                }
            }
        }
    }

    public void AddItemById(int itemId, int stackSize = 1)
    {
        if (!photonView.isMine)
            return;

        Item item = ItemFactory.CreateNewItem(itemId, stackSize);

        if (!inventoryEntities.FirstNullIndexAt().HasValue)
        {
            //Check if we are adding a resource item, if so we check if we have full stacks of the item.
            if (item.GetType() == typeof(Resource))
                if (GetItemAmountById(item.Id) % 64 != 0)
                    FillItemStacksById(itemId, stackSize);

                else
                    ItemFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, item.Id, stackSize);
        }
        else
        {
            if (item.GetType() == typeof(Resource))
            {
                FillItemStacksById(itemId, stackSize);
            }
            else
            {
                AddNewItemStackById(itemId, stackSize);
            }
        }
    }

    public void AddItemAtIndex(int itemId, int index, int stackSize = 1)
    {
        if(index < 0 || inventoryEntities[index] != null)
        {
            Debug.LogError("Inventory -- AddItemAtIndex -- invalid index");
            AddItemById(itemId, stackSize);
        }
        else
        {
            Item item = ItemFactory.CreateNewItem(itemId, stackSize);
            inventoryEntities[index] = item;
            OnItemChangedCallback?.Invoke();
        }
    }

    /// <summary>
    /// Removes the required entities for the craftingRecipe
    /// </summary>
    /// <param name="recipe">The recipe to craft</param>
    /// <returns>Whether there are enough materials to craft this recipe</returns>
    public bool RemoveEntitiesForCrafting(CraftingRecipe recipe)
    {
        for (int i = 0; i < recipe.requiredItems.Count; i++)
        {
            var requiredEntities = recipe.requiredItems[i];
            if (!CheckAmountById(requiredEntities.item.Id, requiredEntities.amount * recipe.amountToCraft))
            {
                Debug.Log($"Not enough {requiredEntities.item.name} to craft {recipe.result.item.name}");
                return false;
            }
        }

        for (int i = 0; i < recipe.requiredItems.Count; i++)
        {
            var requiredEntities = recipe.requiredItems[i];
            RemoveItemById(requiredEntities.item.Id, requiredEntities.amount * recipe.amountToCraft);
        }

        return true;
    }

    public int GetMaxCrafts(CraftingRecipe recipe)
    {
        int maxCrafts = int.MaxValue;
        foreach (var craftingItem in recipe.requiredItems)
        {
            int temp = GetItemAmountById(craftingItem.item.Id) / craftingItem.amount;
            if (temp < maxCrafts)
                maxCrafts = temp;
        }
        return maxCrafts;
    }
}
