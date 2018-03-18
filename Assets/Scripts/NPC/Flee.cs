using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : NPCBaseFSM {

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (Vector3.Distance(Waypoint, NPC.transform.position) < accuracy || Vector3.Distance(Opponent.transform.position, NPC.transform.position) < accuracy)
        {
            Debug.Log("Calculating waypoint...");
            Waypoint = CalculateFleeWaypoint();
        }

        agent.SetDestination(Waypoint);
        // agent.SetDestination(Opponent.transform.position);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(NPC.transform.position);
    }

    Vector3 CalculateFleeWaypoint()
    {
        return Opponent.transform.rotation * NPC.transform.position + Opponent.transform.position + NPC.transform.position * 5;
    }
}
