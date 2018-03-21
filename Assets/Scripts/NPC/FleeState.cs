using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : NPCBaseFSM {

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
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (Vector3.Distance(Waypoint, npc.transform.position) < accuracy || Vector3.Distance(opponent.transform.position, npc.transform.position) < accuracy)
            {
                Waypoint = CalculateFleeWaypoint();
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

    Vector3 CalculateFleeWaypoint()
    {
        var heading = npc.transform.position - opponent.transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;
        direction.y = 0f;
        return npc.transform.position + direction * 10;
    }
}
