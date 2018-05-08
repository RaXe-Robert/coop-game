using UnityEngine;

public interface IInteractable
{
    bool IsInteractable { get; }
    GameObject GameObject { get; }

    bool InRange(Vector3 invokerLocation);
    void Interact(GameObject invoker);

    string TooltipText();
}

