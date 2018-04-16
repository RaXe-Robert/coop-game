using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PhotonView))]
public class BuildableWorldObject : Photon.MonoBehaviour, IInteractable
{
    public Buildable buildable;

    public float interactDistance = 1f;

    #region IInteractable Implementation

    public bool IsInteractable() => interactDistance > 0f;

    public void Interact(Vector3 invokerPosition)
    {
        if (!IsInteractable() || Vector3.Distance(transform.position, invokerPosition) > interactDistance)
            return;

        BuildableInteractionMenu bim = BuildableInteractionMenu.Instance;
        if (bim.TargetInstanceID != GetInstanceID())
            bim.Show(this, new UnityAction[] {
                () => Use(),
                () => Pickup()
            });
        else
            bim.Hide();
    }

    public string TooltipText()
    {
        return $"{buildable.Name}";
    }

    #endregion //IInteractable Implementation

    private void Use()
    {
        Debug.Log("USE");
    }

    private void Pickup()
    {
        Debug.Log("Pickup");
    }


    [PunRPC]
    private void DestroyWorldObject()
    {
        Destroy(gameObject);
    }
}
