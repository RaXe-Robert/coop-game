using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// User Interface class that responsible for showing the ative status effects on the local player.
/// </summary>
public class PlayerStatusEffectsUI : MonoBehaviour
{
    [SerializeField] private GameObject statusEffectDisplayPrefab;

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
        GameObject newStatusEffectDisplay = Instantiate(statusEffectDisplayPrefab, transform);

        StatusEffectDisplay statusEffectDisplay = newStatusEffectDisplay.GetComponent<StatusEffectDisplay>();
        statusEffectDisplay.InitializeDisplay(statusEffect);

        activeStatusEffectsDisplays.Add(statusEffectDisplay);
    }
}
