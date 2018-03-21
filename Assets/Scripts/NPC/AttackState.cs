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
            npc.GetComponent<EnemyNPC>().StartAttack();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            npc.transform.LookAt(opponent.transform.position);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            npc.GetComponent<EnemyNPC>().StopAttack();
        }
    }

}
