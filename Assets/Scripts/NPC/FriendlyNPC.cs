using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyNPC : NPCBase {


    [PunRPC]
    void SetDistance()
    {
        animator.SetFloat("Distance", Vector3.Distance(transform.position, opponent.transform.position));
    }
}


