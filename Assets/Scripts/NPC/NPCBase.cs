﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : Photon.MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName;
    [SerializeField] protected BaseStatsData stats;
    public GameObject Npc { get; private set; }
    public GameObject Target { get; private set; }
    public UnityEngine.AI.NavMeshAgent Agent { get; private set; }
    public Vector3 Waypoint { get; private set; }
    public float NearWaypointRange { get; set; } = 4.0f; // The distance this has to be from the agent waypoint to reach it
    public float InteractionDistance { get; set; } = 4.0f;
    private ItemsToDropComponent itemsToDropComponent;

    private HealthComponent healthComponent;
    private float searchNewTargetCountdown = 1f;
    private Animator animator;

    public delegate void OnNPCKilled();
    public OnNPCKilled OnNPCKilledCallback;
    
    private void Awake()
    {
        Npc = gameObject;
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<HealthComponent>();
        itemsToDropComponent = GetComponent<ItemsToDropComponent>();

        healthComponent.SetValue(stats.maxHealth);
        Agent.speed = stats.movementSpeed;

        foreach (NPCBaseFSM fsm in animator.GetBehaviours<NPCBaseFSM>())
        {
            fsm.NPCScript = this;
        }       
    }

    private void Update()
    {
        if (!PhotonNetwork.isMasterClient) return;
        
        if(searchNewTargetCountdown < 0)
        {
            SetClosestOpponent();
            UpdateDistanceToOpponent();
        }
        searchNewTargetCountdown -= Time.deltaTime;
    }

    /// <summary>
    /// Finds all player objects and calculates which player is the closest to the npc, making this its opponent.
    /// </summary>
    public void SetClosestOpponent()
    {
        PlayerNameTag[] players = FindObjectsOfType<PlayerNameTag>();
        GameObject closestOpponent = null;
        float distance = Mathf.Infinity;

        for (int i = 0; i < players.Length; i++)
        {
            float distanceToObject = Vector3.Distance(players[i].transform.position, transform.position);
            if (distanceToObject < distance)
            {
                closestOpponent = players[i].gameObject;
                distance = distanceToObject;
            }
        }
        Target = closestOpponent;
    }

    /// <summary>
    /// Calculates and sets the waypoint on the hand of the player position. The npc will run the opposite way.
    /// </summary>
    public void SetFleeWaypoint()
    {
        Vector3 heading = Npc.transform.position - Target.transform.position;
        Vector3 direction = heading / heading.magnitude;
        direction.y = 0f;
        Waypoint = Npc.transform.position + direction * 10;
    }

    /// <summary>
    /// Creates and sets a random waypoint away from the npc position.
    /// </summary>
    public void SetRandomWaypoint()
    {
        Waypoint = Npc.transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
    }
    
    /// <summary>
    /// Sets the distance paremeter in the animator.
    /// </summary>
    protected void UpdateDistanceToOpponent()
    {
        animator.SetFloat("Distance", Vector3.Distance(transform.position, Target.transform.position));
    }

    public void TakeDamage(float amount)
    {
        healthComponent.DecreaseValue(amount - stats.defense);

        if (healthComponent.IsDepleted())
        {
            StartCoroutine(PlayDepletedAnimation());
        }
    }

    public bool IsInteractable => true;
    public GameObject GameObject => gameObject;

    public bool InRange(Vector3 invokerPosition) =>
        Vector3.Distance(invokerPosition, transform.position) < InteractionDistance;

    public void Interact(GameObject invoker)
    {
        if (!InRange(invoker.transform.position))
            return;

        var playerMovement = PlayerNetwork.PlayerObject.GetComponent<PlayerMovementController>();
        if (!playerMovement.CanInteract)
            return;
        
        var stats = PlayerNetwork.PlayerObject.GetComponent<StatsComponent>();
        TakeDamage(UnityEngine.Random.Range(stats.MinDamage, stats.MaxDamage));

        playerMovement.AddInteractionTimeout(stats.TimeBetweenAttacks);
    }

    public string TooltipText()
    {
        return npcName;
    }

    protected IEnumerator PlayDepletedAnimation()
    {
        if (animator != null)
        {
            photonView.RPC("CallAnimation", PhotonTargets.All);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length + 1f);
        }

        if (itemsToDropComponent != null) itemsToDropComponent.SpawnItemsOnDepleted();

        photonView.RPC("DestroyObject", PhotonTargets.MasterClient);
    }

    [PunRPC]
    protected void CallAnimation()
    {
        animator.SetBool("isDepleted", true);
    }

    [PunRPC]
    protected void DestroyObject()
    {
        OnNPCKilledCallback?.Invoke();
        PhotonNetwork.Destroy(gameObject);
    }
}
