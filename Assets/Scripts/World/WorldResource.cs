using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldResource : MonoBehaviour, IInteractable
{
    new public string name;
    public float interactDistance = 10f;

    public void Interact(Vector3 invokerPosition)
    {
        if (Vector3.Distance(transform.position, invokerPosition) > interactDistance)
            return;

        PhotonView photonView = PhotonView.Get(this);
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
