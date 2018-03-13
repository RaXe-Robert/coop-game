using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPropertiesUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider = null;
    [SerializeField] private Slider hungerSlider = null;

    public void Update()
    {
        healthSlider.value = PlayerNetwork.PlayerObject?.GetComponent<HealthComponent>()?.Health ?? 0;
        hungerSlider.value = PlayerNetwork.PlayerObject?.GetComponent<HungerComponent>()?.Hunger ?? 0;
    }
}
