using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNPC : NPCBase {
       
    /// <summary>
    /// Handle attacking when the enemy reaches this state.
    /// </summary>
    void Attack()
    {
        Opponent.gameObject.GetComponent<HealthComponent>().Health -= 3;
    }

    /// <summary>
    /// Starts the attack method.
    /// </summary>
    public void StartAttack()
    {
        InvokeRepeating("Attack", 0.5f, 0.5f);
    }

    /// <summary>
    /// Cancels the attack method.
    /// </summary>
    public void StopAttack()
    {
        CancelInvoke("Attack");
    }
}
