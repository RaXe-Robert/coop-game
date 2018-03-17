using UnityEngine;
using System.Collections;

/// <summary>
/// Status effect that modifies health of a given object over time.
/// </summary>
public class HealthStatusEffect : StatusEffectBase
{
    private HealthStatusEffectData healthStatusEffectData;

    private HealthComponent healthComponent;

    public HealthStatusEffect(ScriptableStatusEffectData statusEffect, GameObject gameObj) : base(statusEffect, gameObj)
    {
        healthComponent = gameObj.GetComponent<HealthComponent>();
        healthStatusEffectData = (HealthStatusEffectData)statusEffect;
    }

    public override void OnActivate()
    {
        Debug.Log(this.ToString());
    }

    public override void OnTick(float delta)
    {
        healthComponent.Health += healthStatusEffectData.HealthModification / Duration * delta;
    }

    public override void OnEnd()
    {
    }

    public override string ToString()
    {
        return $"Modifies {gameObj.name} health by {healthStatusEffectData.HealthModification} over {healthStatusEffectData.Duration} seconds.";
    }
}
