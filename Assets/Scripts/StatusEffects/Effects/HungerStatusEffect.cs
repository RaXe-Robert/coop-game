using UnityEngine;
using System.Collections;

/// <summary>
/// Status effect that modifies hunger of a given object over time.
/// </summary>
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
        Debug.Log(this.ToString());
    }

    public override void OnTick(float delta)
    {
        hungerComponent.Hunger += hungerStatusEffectData.HungerModification / Duration * delta;
    }

    public override void OnEnd()
    {
    }

    public override string ToString()
    {
        return $"Modifies {gameObj.name} hunger by {hungerStatusEffectData.HungerModification} over {hungerStatusEffectData.Duration} seconds.";
    }
}
