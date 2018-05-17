using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCBase : Photon.MonoBehaviour, IAttackable, IAttacker
{
    [SerializeField] protected NPCStats stats;
    public GameObject Npc { get; private set; }
    public GameObject Target { get; private set; }
    public Vector3 Waypoint { get; private set; }
    public float NearWaypointRange { get; set; } = 4.0f; // The distance this has to be from the agent waypoint to reach it
    private float searchNewTargetCountdown = 1f;
    public float InteractionDistance { get; set; } = 4.0f;

    //Interfaces
    public GameObject GameObject => gameObject;
    public float Damage => Random.Range(stats.minDamage, stats.maxDamage);
    public string Name => stats.NPCName;
    public string TooltipText => Name;

    //Components
    public NavMeshAgent Agent { get; private set; }

    private ItemsToDropComponent itemsToDropComponent;
    private HealthComponent healthComponent;
    private Animator animator;
    private PlayerCombatController[] players;

    public delegate void OnNPCKilled();
    public OnNPCKilled OnNPCKilledCallback;
    
    private void Awake()
    {
        Npc = gameObject;
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<HealthComponent>();
        itemsToDropComponent = GetComponent<ItemsToDropComponent>();
        players = FindObjectsOfType<PlayerCombatController>();

        healthComponent.SetValue(stats.maxHealth);
        healthComponent.OnDepletedCallback += Die;

        Agent.speed = stats.movementSpeed;

        foreach (NPCBaseFSM fsm in animator.GetBehaviours<NPCBaseFSM>())
        {
            fsm.NPCScript = this;
        }       
    }

    private void Update()
    {
        if (!PhotonNetwork.isMasterClient) return;
        
        if(searchNewTargetCountdown < 0 || Target == null)
        {
            SetClosestOpponent();
            UpdateDistanceToOpponent();
            searchNewTargetCountdown = .25f;
        }
        searchNewTargetCountdown -= Time.deltaTime;
    }

    /// <summary>
    /// Finds all player objects and calculates which player is the closest to the npc, making this its opponent.
    /// </summary>
    public void SetClosestOpponent()
    {
        GameObject closestOpponent = null;
        float distance = Mathf.Infinity;
        players = FindObjectsOfType<PlayerCombatController>();

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].IsDead)
                continue;

            float distanceToObject = Vector3.Distance(players[i].transform.position, transform.position);
            if (distanceToObject < distance)
            {
                closestOpponent = players[i].gameObject;
                distance = distanceToObject;
            }
        }
        Target = closestOpponent;
        if(Target != null)
            animator.SetBool("hasTarget", true);
    }

    /// <summary>
    /// Calculates and sets the waypoint on the hand of the player position. The npc will run the opposite way.
    /// </summary>
    public void SetFleeWaypoint()
    {
        if (Target == null) return;
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
        if (Target == null) return;

        animator.SetFloat("Distance", Vector3.Distance(transform.position, Target.transform.position));
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

    private void Die()
    {
        StartCoroutine(PlayDepletedAnimation());
    }

    public void TakeHit(IAttacker attacker)
    {
        healthComponent.DecreaseValue(attacker.Damage - stats.defense);
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
