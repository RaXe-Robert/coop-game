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
        return interactDistance > 0f;
    }

    public void Interact(Vector3 invokerPosition)
    {
        //TODO: Needs interaction menu
        Debug.Log("Interacting");
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
