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
        Target.gameObject.GetComponent<HealthComponent>().DecreaseValue(Random.Range(stats.minDamage, stats.maxDamage));
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
