using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : NPCBaseFSM {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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
            Npc.Agent.SetDestination(Npc.Opponent.transform.position);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Npc.Agent.SetDestination(Npc.transform.position);
        }
    }
}
