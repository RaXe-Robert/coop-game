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

    public override void Activate()
    {
        Debug.Log(this.ToString());
    }

    protected override void OnTick(float delta)
    {
        healthComponent.IncreaseValue(healthStatusEffectData.HealthModification / Duration * delta);
    }

    public override void End()
    {
    }

    public override string ToString()
    {
        return $"Modifies {gameObj.name} health by {healthStatusEffectData.HealthModification} over {healthStatusEffectData.Duration} seconds.";
    }
}
