using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : NPCBaseFSM {

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(Waypoint, NPC.transform.position) < accuracy || Vector3.Distance(Opponent.transform.position, NPC.transform.position) < accuracy)
        {
            Waypoint = CalculateFleeWaypoint();
        }
        agent.SetDestination(Waypoint);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(NPC.transform.position);
    }

    Vector3 CalculateFleeWaypoint()
    {
        var heading = NPC.transform.position - Opponent.transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;
        direction.y = 0f;
        return NPC.transform.position + direction * 10;
    }
}
