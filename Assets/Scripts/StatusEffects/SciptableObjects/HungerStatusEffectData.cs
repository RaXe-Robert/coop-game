using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "StatusEffects/HungerStatusEffect")]
public class HungerStatusEffectData : ScriptableStatusEffect
{
    [Tooltip("Total hunger modification, can be positive or negative")]
    [SerializeField] private float hungerIncrease;
    public float HungerIncrease { get { return hungerIncrease; } }

    public override StatusEffectBase InitializeStatusEffect(GameObject gameObj)
    {
        return new HungerStatusEffect(this, gameObj);
    }
}
