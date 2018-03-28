using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : Photon.MonoBehaviour {

    public GameObject Npc { get; private set; }
    public GameObject Opponent { get; private set; }
    public UnityEngine.AI.NavMeshAgent Agent { get; private set; }
    public Animator Animator { get; private set; } 
    public Vector3 Waypoint { get; private set; }
    public float NearWaypointRange { get; set; } = 4.0f; // The distance this has to be from the agent waypoint to reach it
    
    private void Awake()
    {
        Npc = gameObject;
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Animator = GetComponent<Animator>();
        foreach (NPCBaseFSM fsm in Animator.GetBehaviours<NPCBaseFSM>())
        {
            fsm.NPCScript = this;
        }       
    }

    private void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            SetClosestOpponent();
            UpdateDistanceToOpponent();
        }
    }

    /// <summary>
    /// Finds all player objects and calculates which player is the closest to the npc, making this its opponent.
    /// </summary>
    public void SetClosestOpponent()
    {
        PlayerNameTag[] players = FindObjectsOfType<PlayerNameTag>();
        GameObject closestOpponent = null;

        for (int i = 0; i < players.Length; i++)
        {
            if(i == 0)
            {
                closestOpponent = players[0].gameObject;
            }
            else
            {
                if (Vector3.Distance(players[i - 1].transform.position, transform.position) > Vector3.Distance(players[i].transform.position, transform.position))
                {
                    closestOpponent = players[i].gameObject;
                }
            }
        }
        Opponent = closestOpponent;
    }

    /// <summary>
    /// Calculates and sets the waypoint on the hand of the player position. The npc will run the opposite way.
    /// </summary>
    public void SetFleeWaypoint()
    {
        Vector3 heading = Npc.transform.position - Opponent.transform.position;
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
        Animator.SetFloat("Distance", Vector3.Distance(transform.position, Opponent.transform.position));
    }
}
