using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : Photon.MonoBehaviour {

    public Animator Animator { get; private set; }
    public GameObject Opponent { get; private set; }
    public UnityEngine.AI.NavMeshAgent Agent { get; private set; }
    public float DistanceToOpponent { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Animator = GetComponent<Animator>();
        foreach (var x in Animator.GetBehaviours<NPCBaseFSM>())
        {
            x.Npc = this;
        }
    }

    private void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            Opponent = GetClosestOpponent();
            DistanceToOpponent = Vector3.Distance(transform.position, Opponent.transform.position);

            UpdateDistanceToOpponent();
        }
    }

    private GameObject GetClosestOpponent()
    {
        PlayerNameTag[] players = FindObjectsOfType<PlayerNameTag>();
        GameObject closestOpponent = null;

        for (int i = 0; i < players.Length; i++)
        {
            closestOpponent = players[0].gameObject;
            if (i != 0)
            {
                if (Vector3.Distance(players[i - 1].transform.position, transform.position) > Vector3.Distance(players[i].transform.position, transform.position))
                {
                    closestOpponent = players[i].gameObject;
                }
            }
        }
        return closestOpponent;
    }

    protected void UpdateDistanceToOpponent()
    {
        Animator.SetFloat("Distance", Vector3.Distance(transform.position, Opponent.transform.position));
    }
}
