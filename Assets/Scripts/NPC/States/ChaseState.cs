using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : NPCBaseFSM {
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            NPCScript.Agent.SetDestination(NPCScript.Opponent.transform.position);
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
