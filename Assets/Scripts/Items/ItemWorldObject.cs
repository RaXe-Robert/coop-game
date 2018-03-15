using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldObject : MonoBehaviour, IInteractable
{
    public Item item;
    public float pickupDistance = 3f;

    private PhotonView photonView;

    public void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void Interact(Vector3 invokerPosition)
    {
        if (Vector3.Distance(transform.position, invokerPosition) > pickupDistance)
            return;

        PlayerNetwork.PlayerObject.GetComponent<Inventory>().AddItemById(item.Id, (item.GetType() == typeof(Resource) ? ((Resource)item).Amount : 1));
        photonView.RPC(nameof(DestroyWorldObject), PhotonTargets.AllBuffered);
    }

    [PunRPC]
    public void DestroyWorldObject()
    {
        Destroy(gameObject);
    }

    public bool IsInteractable()
    {
        return true;
    }
}
