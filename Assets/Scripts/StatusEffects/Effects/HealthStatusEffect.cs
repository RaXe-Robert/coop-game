using UnityEngine;
using System.Collections;

public class HealthStatusEffect : StatusEffectBase
{
    private HealthStatusEffectData healthStatusEffect;

    private HealthComponent healthComponent;

    public HealthStatusEffect(ScriptableStatusEffect statusEffect, GameObject gameObj) : base(statusEffect, gameObj)
    {
        healthComponent = gameObj.GetComponent<HealthComponent>();
        healthStatusEffect = (HealthStatusEffectData)statusEffect;
    }

    public override void OnActivate()
    {
    }

    public override void OnTick(float delta)
    {
        float absoluteHealthEffect = Mathf.Abs(healthStatusEffect.HealthIncrease);
        float percentageOfDuration = delta / timeRemaining * 100;
        
        float healthEffectPerTick = absoluteHealthEffect * percentageOfDuration;

        healthComponent.Health += healthStatusEffect.HealthIncrease / Duration * delta;
    }

    public override void OnEnd()
    {
    }
}
