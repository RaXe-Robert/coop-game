using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNPC : NPCBase
{
    /// <summary>
    /// Handle attacking when the enemy reaches this state.
    /// </summary>
    void Attack()
    {
        if (Target == null)
        {
            GetComponent<Animator>().SetBool("hasTarget", false);
            CancelInvoke("Attack");
            return;
        }

        if(Vector3.Distance(transform.position, Target.transform.position) < 3)
            Target.GetComponent<IAttackable>().TakeHit(this);
    }

    /// <summary>
    /// Starts the attack method.
    /// </summary>
    public void StartAttack()
    {
        InvokeRepeating("Attack", stats.timeBetweenAttacks, stats.timeBetweenAttacks);
    }

    /// <summary>
    /// Cancels the attack method.
    /// </summary>
    public void StopAttack()
    {
        CancelInvoke("Attack");
    }
}
