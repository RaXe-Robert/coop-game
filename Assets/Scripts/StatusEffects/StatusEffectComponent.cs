using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class StatusEffectComponent : MonoBehaviour
{
    public List<StatusEffectBase> CurrentStatusEffects = new List<StatusEffectBase>();

    private void Update()
    {
        foreach (StatusEffectBase statusEffect in CurrentStatusEffects.ToArray())
        {
            statusEffect.Tick(Time.deltaTime);
            if (statusEffect.IsFinished)
            {
                CurrentStatusEffects.Remove(statusEffect);
            }
        }
    }


    public void AddStatusEffect(StatusEffectBase statusEffect)
    {
        CurrentStatusEffects.Add(statusEffect);
        statusEffect.OnActivate();
    }

    public void AddStatusEffect(List<ScriptableStatusEffect> statusEffects)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            var statusEffect = CreateStatusEffect(statusEffects[i]);
            if (statusEffect == null) continue;

            CurrentStatusEffects.Add(statusEffect);
        }
    }

    private StatusEffectBase CreateStatusEffect(ScriptableStatusEffect statusEffect)
    {
        var statusEffectType = statusEffect.GetType();
        if (statusEffectType == typeof(HealthStatusEffectData))
        {
            return new HealthStatusEffect(statusEffect, gameObject);

        }
        else if (statusEffectType == typeof(HungerStatusEffectData))
        {
            return new HungerStatusEffect(statusEffect, gameObject);
        }

        return null;
    }
}
