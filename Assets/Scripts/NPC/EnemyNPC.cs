using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNPC : NPCBase {
       
    void Attack()
    {
       // Debug.Log("Npc is attacking the player");
    }

    public void StartAttack()
    {
        InvokeRepeating("Attack", 0.5f, 0.5f);
    }

    public void StopAttack()
    {
        CancelInvoke("Attack");
    }

    [PunRPC]
    void SetDistance()
    {
        Animator.SetFloat("Distance", Vector3.Distance(transform.position, Opponent.transform.position));
    }
}
