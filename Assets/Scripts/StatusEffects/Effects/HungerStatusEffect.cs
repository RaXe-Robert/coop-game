using UnityEngine;
using System.Collections;

/// <summary>
/// Status effect that modifies hunger of a given object over time.
/// </summary>
public class HungerStatusEffect : StatusEffectBase
{
    private HungerStatusEffectData hungerStatusEffectData;

    private HungerComponent hungerComponent;

    public HungerStatusEffect(ScriptableStatusEffectData statusEffect, GameObject gameObj) : base(statusEffect, gameObj)
    {
        hungerComponent = gameObj.GetComponent<HungerComponent>();
        hungerStatusEffectData = (HungerStatusEffectData)statusEffect;
    }

    public override void OnActivate()
    {
        hungerComponent.HungerDegenerationActive = false;
    }

    public override void OnTick(float delta)
    {
        hungerComponent.Hunger += hungerStatusEffectData.HungerModification / Duration * delta;
    }

    public override void OnEnd()
    {
        hungerComponent.HungerDegenerationActive = true;
    }

    public override string ToString()
    {
        return $"Modifies {gameObj.name} hunger by {hungerStatusEffectData.HungerModification} over {hungerStatusEffectData.Duration} seconds.";
    }
}
