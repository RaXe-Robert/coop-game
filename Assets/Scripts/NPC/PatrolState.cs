using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : NPCBaseFSM {
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }   

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(Waypoint, NPC.transform.position) < accuracy || !agent.hasPath)
        {
            Waypoint = CreateWaypoint();
        } 
        
        agent.SetDestination(Waypoint);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(NPC.transform.position);
    }

    Vector3 CreateWaypoint()
    {
        return NPC.transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
    }

   
}
