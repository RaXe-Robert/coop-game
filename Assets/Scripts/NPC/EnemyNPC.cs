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
        if(Vector3.Distance(transform.position, Target.transform.position) < 3)
        {
            //TODO: this should be changed
            Target.gameObject.GetComponent<HealthComponent>().DecreaseValue((Random.Range(stats.minDamage, stats.maxDamage) - Target.gameObject.GetComponent<PlayerStatsComponent>().Defense));
        }
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
