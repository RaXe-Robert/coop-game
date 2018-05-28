using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Assets.Scripts.Map_Generation;

[RequireComponent(typeof(Animator), typeof(ItemsToDropComponent))]
public class WorldResource : MonoBehaviour, IInteractable
{
    public new string name;
    public ToolType requiredToolToHarvest;
    public float interactDistance = 5f;

    [SerializeField] private GameObject spawnOnDepleted;
    [SerializeField] private float maxHealth = 100f;

    public Animator Animator { get; private set; }
    public ItemsToDropComponent ItemsToDrop { get; private set; }

    public TerrainChunk TerrainChunk { get; private set; }
    public double Id { get; private set; }

    public string Name => name;
    public float MaxHealth => maxHealth;
    public ToolType RequiredToolToHarvest => requiredToolToHarvest;
    public float InteractDistance => interactDistance;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        ItemsToDrop = GetComponent<ItemsToDropComponent>();
    }

    public void Setup(TerrainChunk terrainChunk, double id)
    {
        this.TerrainChunk = terrainChunk;
        this.Id = id;
    }

    #region IInteractable Implementation

    public bool IsInteractable => true;
    public GameObject GameObject => gameObject;

    public bool InRange(Vector3 invokerPosition) => Vector3.Distance(invokerPosition, transform.position) < interactDistance;

    public void Interact(GameObject invoker)
    {
        if (!InRange(invoker.transform.position))
            return;

        var playerMovement = PlayerNetwork.LocalPlayer.GetComponent<PlayerMovementController>();
        if (!playerMovement.CanInteract)
        {
            WorldNotificationsManager.Instance.ShowLocalNotification(new WorldNotificationArgs(transform.position, "Not ready yet", 1));
            return;
        }

        var stats = PlayerNetwork.LocalPlayer.GetComponent<PlayerStatsComponent>();
        playerMovement.AddInteractionTimeout(stats.TimeBetweenResourceHits);

        var equipmentManager = PlayerNetwork.LocalPlayer.GetComponent<EquipmentManager>();
        if (!equipmentManager.HasToolEquipped(requiredToolToHarvest))
        {
            WorldNotificationsManager.Instance.ShowLocalNotification(new WorldNotificationArgs(transform.position, "Not ready yet", 1));
            return;
        }

        WorldResourceManager.Instance.DecreaseHealth(this, TerrainChunk, 50f);
    }

    public string TooltipText => $"{name} \nRequires {requiredToolToHarvest}";

    #endregion //IInteractable Implementation
}
