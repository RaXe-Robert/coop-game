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
            new UnityAction(OpenCampfire),
            new UnityAction(CloseCampfire)
        };
    }

    private void ToggleFire()
    {
        IsBurning = !IsBurning;

        if (fireParticlesPrefab)
        {
            if (IsBurning)
            {
                activeFireParticles = Instantiate(fireParticlesPrefab, transform);
            }
            else
                Destroy(activeFireParticles);
        }
    }

    private void OpenCampfire()
    {
        Debug.Log("Opening");

        //The toggle fire should be implemented when something is cooking.
        ToggleFire();
    }

    private void CloseCampfire()
    {
        Debug.Log("Opening");
    }
}
