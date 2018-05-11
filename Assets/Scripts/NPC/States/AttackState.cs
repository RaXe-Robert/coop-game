using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : NPCBaseFSM
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            ((EnemyNPC)NPCScript).StartAttack();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient && NPCScript.Target != null)
        {
            NPCScript.Npc.transform.LookAt(NPCScript.Target.transform.position);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.isMasterClient)
        {
            ((EnemyNPC)NPCScript).StopAttack();
        }
    }
}
