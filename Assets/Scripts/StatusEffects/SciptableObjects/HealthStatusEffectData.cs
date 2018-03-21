using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "StatusEffects/HealthStatusEffect")]
public class HealthStatusEffectData : ScriptableStatusEffectData
{
    [Tooltip("Total health modification, can be positive or negative")]
    [SerializeField] private float healthModification;
    public float HealthModification { get { return healthModification; } }

    public override StatusEffectBase InitializeStatusEffect(GameObject gameObj)
    {
        return new HealthStatusEffect(this, gameObj);
    }
}
