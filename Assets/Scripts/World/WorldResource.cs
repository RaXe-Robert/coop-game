using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Assets.Scripts.Map_Generation;

[RequireComponent(typeof(Animator), typeof(ItemsToDropComponent))]
public class WorldResource : MonoBehaviour, IInteractable
{
    [SerializeField] private new string name;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private ToolType requiredToolToHarvest;
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private GameObject spawnOnDepleted;

    private Animator animator;
    private ItemsToDropComponent itemsToDrop;

    public TerrainChunk TerrainChunk { get; private set; }
    public double ID { get; private set; }

    public string Name => name;
    public float MaxHealth => maxHealth;
    public ToolType RequiredToolToHarvest => requiredToolToHarvest;
    public float InteractDistance => interactDistance;

    private void Start()
    {
        animator = GetComponent<Animator>();
        itemsToDrop = GetComponent<ItemsToDropComponent>();
    }

    public void Setup(TerrainChunk terrainChunk, double id)
    {
        this.TerrainChunk = terrainChunk;
        this.ID = id;
    }
    
    private IEnumerator PlayDepletedAnimation()
    {
        if (animator != null)
        {
            //photonView.RPC("CallAnimation", PhotonTargets.All);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length + 1f);
        }

        itemsToDrop?.SpawnItemsOnDepleted();

       // photonView.RPC("DestroyObject", PhotonTargets.MasterClient);
    }

    #region IInteractable Implementation

    public bool IsInteractable => true;
    public GameObject GameObject => gameObject;

    public bool InRange(Vector3 invokerPosition) => Vector3.Distance(invokerPosition, transform.position) < interactDistance;

    public void Interact(GameObject invoker)
    {
        if (!InRange(invoker.transform.position))
            return;

        var playerMovement = PlayerNetwork.PlayerObject.GetComponent<PlayerMovementController>();
        if (!playerMovement.CanInteract)
        {
            WorldNotificationsManager.Instance.ShowNotification(new WorldNotificationArgs(transform.position, "Not ready yet", 1), true);
            return;
        }

        var stats = PlayerNetwork.PlayerObject.GetComponent<StatsComponent>();
        playerMovement.AddInteractionTimeout(stats.TimeBetweenResourceHits);

        var equipmentManager = PlayerNetwork.PlayerObject.GetComponent<EquipmentManager>();
        if (!equipmentManager.HasToolEquipped(requiredToolToHarvest))
        {
            WorldNotificationsManager.Instance.ShowNotification(new WorldNotificationArgs(transform.position, "Not ready yet", 1), true);
            return;
        }

        WorldResourceManager.Instance.DecreaseHealth(this, TerrainChunk, 50f);
    }

    public string TooltipText()
    {
        return $"{name} \nRequires {requiredToolToHarvest}";
    }

    #endregion //IInteractable Implementation

    [PunRPC]
    void CallAnimation()
    {
        animator.SetBool("isDepleted", true);
    }

    [PunRPC]
    void DestroyObject()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
