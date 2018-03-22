using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : NPCBaseFSM
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            ((EnemyNPC)Npc).StartAttack();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            Npc.transform.LookAt(Npc.Opponent.transform.position);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            ((EnemyNPC)Npc).StopAttack();
        }
    }
}
