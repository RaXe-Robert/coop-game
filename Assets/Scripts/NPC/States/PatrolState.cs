using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : NPCBaseFSM {
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (Vector3.Distance(NPCScript.Waypoint, NPCScript.Npc.transform.position) < NPCScript.NearWaypointRange || !NPCScript.Agent.hasPath)
            {
                NPCScript.SetRandomWaypoint();
            }
            NPCScript.Agent.SetDestination(NPCScript.Waypoint);
        }
    }

     public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            NPCScript.Agent.SetDestination(NPCScript.Npc.transform.position);
        }
    }
}
