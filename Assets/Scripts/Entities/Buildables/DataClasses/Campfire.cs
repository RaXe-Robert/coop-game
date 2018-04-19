using System.Collections;

using UnityEngine;
using UnityEngine.Events;

public class Campfire : BuildableBase
{
    private GameObject fireParticlesPrefab;

    private GameObject activeFireParticles = null;

    public bool IsBurning { get; private set; } = false;

    public Campfire(CampfireData campfireData) : base(campfireData)
    {
        fireParticlesPrefab = campfireData.FireParticles;
    }

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(ToggleFire),
            new UnityAction(OpenCookingMenu)
        };
    }

    private void ToggleFire()
    {
        // If null the action will be cancelled
        if (BuildableInteractionMenu.Instance?.Target == null)
            return;

        IsBurning = !IsBurning;

        if (fireParticlesPrefab)
        {
            if (IsBurning)
            {
                activeFireParticles = GameObject.Instantiate(fireParticlesPrefab, BuildableInteractionMenu.Instance.Target.transform);
            }
            else
                GameObject.Destroy(activeFireParticles);
        }
    }
    private void OpenCookingMenu()
    {
        Debug.Log("Opening");
    }
}
