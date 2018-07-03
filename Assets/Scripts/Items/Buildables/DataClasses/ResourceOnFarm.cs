using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceOnFarm : Photon.MonoBehaviour, IInteractable {

    Farm farm;
    public float InteractDistance => farm.interactDistance;
    
    public void Initialize(Farm f)
    {
        farm = f;
        farm.resourceOnFarm = gameObject;
    }

    public bool IsInteractable => true;
    public GameObject GameObject => gameObject;

    public string TooltipText => $"A melon for you and your companions";

    public bool InRange(Vector3 invokerPosition) => Vector3.Distance(invokerPosition, transform.position) < InteractDistance;

    public void Interact(GameObject invoker, Item item)
    {
        if (!InRange(invoker.transform.position))
            return;

        if (!farm.isGrowing)
            FarmResource();
        else
            FeedUI.Instance.AddFeedItem("Take your time: " + farm.timeLeft + " s", feedType: FeedItem.Type.Fail);
    }

    public void FarmResource()
    {
        farm.Harvest();
    }
}
