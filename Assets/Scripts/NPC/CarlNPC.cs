using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlNPC : MonoBehaviour {
       
    private Animator animator;
        
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	void Update ()
    {
        animator.SetFloat("Distance", Vector3.Distance(transform.position, PlayerNetwork.PlayerObject.transform.position));	
	}

    void Attack()
    {
        Debug.Log("Npc is attacking the player");
    }

    public void StartAttack()
    {
        InvokeRepeating("Attack", 0.5f, 0.5f);
    }

    public void StopAttack()
    {
        CancelInvoke("Attack");
    }
}
