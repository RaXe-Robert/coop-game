﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldResource : Photon.MonoBehaviour, IInteractable
{
    new public string name;
    public float interactDistance = 5f;
    [SerializeField] GameObject spawnOnDepleted;
    [SerializeField] HealthComponent healthComponent;
    [SerializeField] ItemsToDropComponent itemsToDrop;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact(Vector3 invokerPosition)
    {
        if (Vector3.Distance(transform.position, invokerPosition) > interactDistance)
            return;

        if (!healthComponent.IsDepleted()){

            healthComponent.RPCReduceHealth(50);
        }
        else
        {
            StartCoroutine(PlayDepletedAnimation());
        }
    }

    private IEnumerator PlayDepletedAnimation()
    {

        if (animator != null)
        {
            animator.SetBool("isDepleted", true);

            AnimatorStateInfo animation = animator.GetCurrentAnimatorStateInfo(0);

            yield return new WaitForSeconds(animation.length);
        }
        
        if (spawnOnDepleted != null)
        {
            itemsToDrop.SpawnObjectOnParent(spawnOnDepleted);
        }

        itemsToDrop.SpawnObjectsOnDepleted();

        photonView.RPC("DestroyObject", PhotonTargets.MasterClient);
    }

    public bool IsInteractable()
    {
        return true;
    }

   [PunRPC]
   void DestroyObject()
   {          
       PhotonNetwork.Destroy(gameObject);
   }   
}
