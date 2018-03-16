using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "StatusEffects/HungerStatusEffect")]
public class HungerStatusEffectData : ScriptableStatusEffect
{
    [SerializeField] private float hungerIncrease;

    public float HungerIncrease { get { return hungerIncrease; } }

    public override StatusEffectBase InitializeStatusEffect(GameObject gameObj)
    {
        return new HungerStatusEffect(this, gameObj);
    }
}
