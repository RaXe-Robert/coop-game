using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "StatusEffects/HungerStatusEffect")]
public class HungerStatusEffectData : ScriptableStatusEffectData
{
    [Tooltip("Total hunger modification, can be positive or negative")]
    [SerializeField] private float hungerModification;
    public float HungerModification { get { return hungerModification; } }

    public override StatusEffectBase InitializeStatusEffect(GameObject gameObj)
    {
        return new HungerStatusEffect(this, gameObj);
    }
}
