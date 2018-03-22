using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyNPC : NPCBase {


    [PunRPC]
    void SetDistance()
    {
        Animator.SetFloat("Distance", Vector3.Distance(transform.position, Opponent.transform.position));
    }
}


