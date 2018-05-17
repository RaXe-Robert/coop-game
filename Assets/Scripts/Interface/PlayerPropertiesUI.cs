using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Responsible for updating the UI related to player properties.
/// </summary>
public class PlayerPropertiesUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider = null;
    [SerializeField] private Slider hungerSlider = null;
    
    public void Start()
    {
        HealthComponent playerHealthComponent = PlayerNetwork.LocalPlayer?.GetComponent<HealthComponent>();
        UpdateHealthSlider(playerHealthComponent.Value);
        playerHealthComponent.OnValueChangedCallback += UpdateHealthSlider;

        HungerComponent playerHungerComponent = PlayerNetwork.LocalPlayer?.GetComponent<HungerComponent>();
        UpdateHungerSlider(playerHungerComponent.Value);
        playerHungerComponent.OnValueChangedCallback += UpdateHungerSlider;
    }
    
    public void UpdateHealthSlider(float value)
    {
        healthSlider.value = value;
    }

    public void UpdateHungerSlider(float value)
    {
        hungerSlider.value = value;
    }
}
