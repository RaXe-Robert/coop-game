using UnityEngine;

public interface IInteractable
{
    void Interact(Vector3 invokerPosition);
    bool IsInteractable();
}

