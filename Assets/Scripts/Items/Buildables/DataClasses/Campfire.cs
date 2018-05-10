using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Campfire : BuildableWorldObject
{
    [SerializeField] private GameObject fireParticlesPrefab;
    private GameObject activeFireParticles = null;

    public bool IsBurning { get; private set; } = false;

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
                activeFireParticles = Instantiate(fireParticlesPrefab, BuildableInteractionMenu.Instance.Target.transform);
            }
            else
                Destroy(activeFireParticles);
        }
    }

    private void OpenCookingMenu()
    {
        Debug.Log("Opening");
    }
}
