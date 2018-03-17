using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// User Interface class that responsible for showing the ative status effects on the local player.
/// </summary>
public class PlayerStatusEffectsUI : MonoBehaviour
{
    [SerializeField] private StatusEffectDisplay statusEffectDisplayResource;

    private List<StatusEffectDisplay> activeStatusEffectsDisplays;

    private StatusEffectComponent statusEffectComponent;

    private void Start()
    {
        activeStatusEffectsDisplays = new List<StatusEffectDisplay>();

        statusEffectComponent = PlayerNetwork.PlayerObject?.GetComponent<StatusEffectComponent>();
        statusEffectComponent.OnstatusEffectAdded += AddNewStatusEffectDisplay;
    }
    
    private void AddNewStatusEffectDisplay(StatusEffectBase statusEffect)
    {
        StatusEffectDisplay statusEffectDisplay = Instantiate(statusEffectDisplayResource, transform);
        statusEffectDisplay.InitializeDisplay(statusEffect);

        activeStatusEffectsDisplays.Add(statusEffectDisplay);
    }
}
