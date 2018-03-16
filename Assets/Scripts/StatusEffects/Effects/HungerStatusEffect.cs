﻿using UnityEngine;
using System.Collections;

public class HungerStatusEffect : StatusEffectBase
{
    private HungerStatusEffectData hungerStatusEffectData;

    private HungerComponent hungerComponent;

    public HungerStatusEffect(ScriptableStatusEffect statusEffect, GameObject gameObj) : base(statusEffect, gameObj)
    {
        hungerComponent = gameObj.GetComponent<HungerComponent>();
        hungerStatusEffectData = (HungerStatusEffectData)statusEffect;
    }

    public override void OnActivate()
    {
    }

    public override void OnTick(float delta)
    {
        hungerComponent.Hunger += hungerStatusEffectData.HungerIncrease / Duration * delta;
    }

    public override void OnEnd()
    {
    }
}
