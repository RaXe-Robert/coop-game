using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class BuildableWorldObject : Photon.MonoBehaviour, IInteractable
{
    public Buildable buildable;
    public float interactDistance = 1f;
    
    #region IInteractable Implementation

    public bool IsInteractable()
    {
        return true;
    }

    public void Interact(Vector3 invokerPosition)
    {
        //TODO: Needs interaction menu
        Debug.Log("Interacting");
        
        /*
        if (Vector3.Distance(transform.position, invokerPosition) > interactDistance)
        {
            PlayerNetwork.PlayerObject.GetComponent<PlayerMovementController>().ItemToPickup = this.gameObject;
            return;
        }

        PlayerNetwork.PlayerObject.GetComponent<Inventory>().AddItemById(buildable.Id, buildable.StackSize);
        photonView.RPC(nameof(DestroyWorldObject), PhotonTargets.AllBuffered);*/
    }

    public string TooltipText()
    {
        return $"";
    }

    #endregion //IInteractable Implementation

    [PunRPC]
    private void DestroyWorldObject()
    {
        Destroy(gameObject);
    }
}
