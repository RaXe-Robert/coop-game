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

    public override void Activate()
    {
        hungerComponent.HungerDegenerationActive = false;
    }

    protected override void OnTick(float delta)
    {
        hungerComponent.IncreaseValue(hungerStatusEffectData.HungerModification / Duration * delta);
    }

    public override void End()
    {
        hungerComponent.HungerDegenerationActive = true;
    }

    public override string ToString()
    {
        return $"Modifies {gameObj.name} hunger by {hungerStatusEffectData.HungerModification} over {hungerStatusEffectData.Duration} seconds.";
    }
}
