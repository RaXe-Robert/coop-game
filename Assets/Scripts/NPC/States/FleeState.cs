using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : NPCBaseFSM {

    [SerializeField] private float fleeRange = 4f; // The range away from the opponent for it to start fleeing.

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient && NPCScript.Target != null)
        {
            if (Vector3.Distance(NPCScript.Waypoint, NPCScript.Npc.transform.position) < NPCScript.NearWaypointRange || Vector3.Distance(NPCScript.Target.transform.position, NPCScript.Npc.transform.position) < fleeRange)
            {
                NPCScript.SetFleeWaypoint();
            }
            if (NPCScript.Agent.isOnNavMesh)
                NPCScript.Agent.SetDestination(NPCScript.Waypoint);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient && NPCScript.Agent.isOnNavMesh)
        {
            NPCScript.Agent.SetDestination(NPCScript.Npc.transform.position);
        }
    }
}
