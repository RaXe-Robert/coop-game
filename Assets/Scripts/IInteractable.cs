using UnityEngine;

public interface IInteractable
{
    bool IsInteractable();
    void Interact(Vector3 invokerPosition);

    bool HasTooltip();
    string TooltipText();
}

