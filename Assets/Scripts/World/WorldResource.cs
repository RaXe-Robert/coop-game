using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldResource : MonoBehaviour, IInteractable
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
            if (animator != null)
            {
                animator.SetBool("isDepleted", true);
                //Find a way to wait for animation here
            }

            if (spawnOnDepleted != null)
            {
                itemsToDrop.SpawnObjectOnParent(spawnOnDepleted);
            }

            itemsToDrop.SpawnObjectsOnDepleted();

            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("DestroyObject", PhotonTargets.MasterClient);                     
        }
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
