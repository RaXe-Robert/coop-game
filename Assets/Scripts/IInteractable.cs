using UnityEngine;

public interface IInteractable : ITooltip
{
    bool IsInteractable { get; }
    GameObject GameObject { get; }

    bool InRange(Vector3 invokerLocation);
    void Interact(GameObject invoker, Item item);
}

