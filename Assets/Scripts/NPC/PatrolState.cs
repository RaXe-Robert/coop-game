using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : NPCBaseFSM {

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (Vector3.Distance(Waypoint, npc.transform.position) < accuracy || !agent.hasPath)
            {
                Waypoint = CreateWaypoint();
            }

            agent.SetDestination(Waypoint);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            agent.SetDestination(npc.transform.position);
        }
    }

    Vector3 CreateWaypoint()
    {
        return npc.transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
    }

   
}
