using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Map_Generation;

[RequireComponent(typeof(CapsuleCollider), typeof(ItemsToDropComponent))]
public class WorldResource : MonoBehaviour, IInteractable
{
    public new string name;
    public ToolType requiredToolToHarvest;
    public float interactDistance = 5f;
    public SoundManager.AttackSound attackSound = SoundManager.AttackSound.WOOD;

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

    public void Interact(GameObject invoker, Item item)
    {
        if (!InRange(invoker.transform.position))
            return;

        SoundManager.Instance.PlayAttackSound(attackSound);

        var playerMovement = PlayerNetwork.LocalPlayer.GetComponent<PlayerMovementController>();
        if (!playerMovement.CanInteract)
        {
            WorldNotificationsManager.Instance.ShowLocalNotification(new WorldNotificationArgs(transform.position, "Not ready yet", 1));
            return;
        }
        
        if(item?.GetType() == typeof(Tool))
        {
            Tool tool = item as Tool;
            if(tool.ToolType == RequiredToolToHarvest)
                WorldResourceManager.Instance.DecreaseHealth(this, TerrainChunk, 50f);
            else
                WorldNotificationsManager.Instance.ShowLocalNotification(new WorldNotificationArgs(transform.position, $"Requires a {RequiredToolToHarvest} to harvest", 1));
        }        
        else
            WorldNotificationsManager.Instance.ShowLocalNotification(new WorldNotificationArgs(transform.position, $"Requires a {RequiredToolToHarvest} to harvest", 1));
    }

    public string TooltipText => $"{name} \nRequires {requiredToolToHarvest}";

    #endregion //IInteractable Implementation
}
