using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Component that is responsible for all status effects on a certain object.
/// </summary>
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
    
    /// <summary>
    /// Add a new status effect to this component.
    /// </summary>
    /// <param name="statusEffects">The status effect to add</param>
    public void AddStatusEffect(StatusEffectBase statusEffect)
    {
        CurrentStatusEffects.Add(statusEffect);
        statusEffect.OnActivate();
    }

    /// <summary>
    /// Add a list of new status effects to this component.
    /// </summary>
    /// <param name="statusEffects">The list of status effects to add</param>
    public void AddStatusEffect(List<ScriptableStatusEffect> statusEffects)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            var statusEffect = statusEffects[i].InitializeStatusEffect(gameObject);
            CurrentStatusEffects.Add(statusEffect);
        }
    }
}
