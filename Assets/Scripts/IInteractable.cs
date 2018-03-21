using UnityEngine;

public interface IInteractable
{
    bool IsInteractable();
    void Interact(Vector3 invokerPosition);

    string TooltipText();
}

