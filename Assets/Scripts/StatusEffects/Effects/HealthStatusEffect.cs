using UnityEngine;
using System.Collections;

/// <summary>
/// Status effect that modifies health of a given object over time.
/// </summary>
public class HealthStatusEffect : StatusEffectBase
{
    private HealthStatusEffectData healthStatusEffectData;

    private HealthComponent healthComponent;

    public HealthStatusEffect(ScriptableStatusEffect statusEffect, GameObject gameObj) : base(statusEffect, gameObj)
    {
        healthComponent = gameObj.GetComponent<HealthComponent>();
        healthStatusEffectData = (HealthStatusEffectData)statusEffect;
    }

    public override void OnActivate()
    {
    }

    public override void OnTick(float delta)
    {
        healthComponent.Health += healthStatusEffectData.HealthIncrease / Duration * delta;
    }

    public override void OnEnd()
    {
    }
}
