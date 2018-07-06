using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ExitGames.Client.Photon;

/// <summary>
/// Responsible for updating the UI related to player properties.
/// </summary>
public class PlayerPropertiesUI : MonoBehaviour
{
    [SerializeField] private StatusBarProgress healthSlider = null;
    [SerializeField] private StatusBarProgress hungerSlider = null;
    
    public void Start()
    {
        HealthComponent playerHealthComponent = PlayerNetwork.LocalPlayer?.GetComponent<HealthComponent>();
        UpdateHealthSlider(playerHealthComponent.Value);
        playerHealthComponent.OnValueChangedCallback += UpdateHealthSlider;

        HungerComponent playerHungerComponent = PlayerNetwork.LocalPlayer?.GetComponent<HungerComponent>();
        UpdateHungerSlider(playerHungerComponent.Value);
        playerHungerComponent.OnValueChangedCallback += UpdateHungerSlider;

        ExitGames.Client.Photon.Hashtable playerHealthAndHunger = new ExitGames.Client.Photon.Hashtable() { { "Health", playerHealthComponent.Value },{ "Hunger", playerHungerComponent.Value } };
        PhotonNetwork.SetPlayerCustomProperties(playerHealthAndHunger);
    }
    
    public void UpdateHealthSlider(float value)
    {
        healthSlider.SetValue(value / 100);
        ExitGames.Client.Photon.Hashtable playerHealth = new ExitGames.Client.Photon.Hashtable() { { "Health", value }};
        PhotonNetwork.SetPlayerCustomProperties(playerHealth);
    }

    public void UpdateHungerSlider(float value)
    {
        hungerSlider.SetValue(value / 100);
        ExitGames.Client.Photon.Hashtable playerHunger = new ExitGames.Client.Photon.Hashtable() { { "Hunger", value } };
        PhotonNetwork.SetPlayerCustomProperties(playerHunger);        
    }
}
