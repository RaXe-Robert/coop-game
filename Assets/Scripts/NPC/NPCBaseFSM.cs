using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBaseFSM : StateMachineBehaviour {

    public GameObject npc;
    public GameObject opponent;
    public UnityEngine.AI.NavMeshAgent agent;
    public Vector3 Waypoint;
    public float accuracy = 1.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        npc = animator.gameObject;
        opponent = GetClosestOpponent();
        agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
     }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        opponent = GetClosestOpponent();
    }

    GameObject GetClosestOpponent()
    {
        PlayerNameTag[] players = FindObjectsOfType<PlayerNameTag>();
        GameObject ClosestOpponent = players[0].gameObject;
        for (int i = 0; i < players.Length; i++)
        {
            if(i != 0)
            {
                if (Vector3.Distance(players[i-1].gameObject.transform.position, npc.transform.position) > Vector3.Distance(players[i].gameObject.transform.position, npc.transform.position))
                {
                    ClosestOpponent = players[i].gameObject;
                }
            }           
        }
        return ClosestOpponent;
    }


}
