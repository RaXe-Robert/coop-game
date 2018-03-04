using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldObject : MonoBehaviour {
    public Item item;

    public void Interact()
    {
        if (PlayerNetwork.PlayerObject.GetComponent<Inventory>().AddItem(item))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
