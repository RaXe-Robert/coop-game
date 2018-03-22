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

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (Vector3.Distance(Waypoint, Npc.transform.position) < Range || Vector3.Distance(Npc.Opponent.transform.position, Npc.transform.position) < Range)
            {
                Waypoint = CalculateFleeWaypoint();
            }
            Npc.Agent.SetDestination(Waypoint);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Npc.Agent.SetDestination(Npc.transform.position);
        }
    }

    private Vector3 CalculateFleeWaypoint()
    {
        Vector3 heading = Npc.transform.position - Npc.Opponent.transform.position;
        Vector3 direction = heading / heading.magnitude;
        direction.y = 0f;
        return Npc.transform.position + direction * 10;
    }
}
