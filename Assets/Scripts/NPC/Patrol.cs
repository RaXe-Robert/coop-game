using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : NPCBaseFSM {
    Vector3 Waypoint;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }   

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(Waypoint, NPC.transform.position) < accuracy)
        {
            Waypoint = createWaypoint();
        }

        agent.SetDestination(Waypoint);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    Vector3 createWaypoint()
    {
        return NPC.transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
    }

   
}
