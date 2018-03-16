using UnityEngine;
using System.Collections;

public class HungerStatusEffect : StatusEffectBase
{
    private HungerStatusEffectData healthStatusEffect;

    private HungerComponent hungerComponent;

    public HungerStatusEffect(ScriptableStatusEffect statusEffect, GameObject gameObj) : base(statusEffect, gameObj)
    {
        hungerComponent = gameObj.GetComponent<HungerComponent>();
        healthStatusEffect = (HungerStatusEffectData)statusEffect;
    }

    public override void OnActivate()
    {
    }

    public override void OnTick(float delta)
    {
        hungerComponent.Hunger += healthStatusEffect.HungerIncrease * delta;
    }

    public override void OnEnd()
    {
    }
}
