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

        PhotonNetwork.Destroy(gameObject);
    }

    public bool IsInteractable()
    {
        return true;
    }
}
