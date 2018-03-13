using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour {

    [SerializeField] private int health;
    
    public void RPCReduceHealth(int amount)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("ReduceHealth", PhotonTargets.All, amount);
    }

    [PunRPC]
    void ReduceHealth(int amount)
    {        
        health -= amount;
    }

    void AddHealth(int amount)
    {
        health += amount;
    }

    public int GetHealth()
    {
        return health;
    }

    public int SetHealth(int amount)
    {
        health = amount;
        return health;
    }

    public bool IsDepleted()
    {
        if (health <= 0)
        {
            return true;
        }
        return false;
    }

}
