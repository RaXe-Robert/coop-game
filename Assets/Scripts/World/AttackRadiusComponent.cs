using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRadiusComponent : MonoBehaviour {
    
    [SerializeField] private HealthComponent health;
    [SerializeField] private ItemsToDropComponent itemsToDrop;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private float radius;
    private bool clickable = false;
    
	void Start () {
        sphereCollider.radius = radius;
	}

    void OnTriggerEnter(Collider other)
    {        
       if (other.gameObject.name.Substring(0, 6) == "Player")
       {
           clickable = true;
       }
    }

    void OnMouseDown()
    {
        if (!clickable)
        {
            return;
        }
        
        PhotonView photonView = PhotonView.Get(this);
        health.RPCReduceHealth(100);
        if(health.IsBroken())
        {
            itemsToDrop.SpawnMultipleObjects();            
            photonView.RPC("DestroyObject", PhotonTargets.MasterClient);
        }
        Debug.Log(health.GetHealth());
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Substring(0, 6) == "Player")
        {
            clickable = false;
        }
    }

    [PunRPC]
    void DestroyObject()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    

}
