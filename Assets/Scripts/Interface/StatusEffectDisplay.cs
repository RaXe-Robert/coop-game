using UnityEngine;
using UnityEngine.UI;

public class StatusEffectDisplay : MonoBehaviour
{
    private StatusEffectBase statusEffect;

    [SerializeField] private Text timeRemaining;
    [SerializeField] private Image image;

    public void InitializeDisplay(StatusEffectBase statusEffect)
    {
        this.statusEffect = statusEffect;

        timeRemaining.text = statusEffect.TimeRemaining.ToString();
        image.sprite = statusEffect.StatusEffectData.Icon;

        statusEffect.OnStatusEffectTick += UpdateDisplay;
    }

    private void UpdateDisplay()
    {
        if (statusEffect.IsFinished)
        {
            statusEffect.OnStatusEffectTick -= UpdateDisplay;
            Destroy(gameObject);
        }

        timeRemaining.text = statusEffect.TimeRemaining.ToString("F0");
    }
}
