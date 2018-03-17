using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Component that is responsible for all status effects on a certain object.
/// </summary>
public class StatusEffectComponent : MonoBehaviour
{
    [SerializeField] private bool MergeEffectsOfSameType = true;
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
    public void AddStatusEffect(ScriptableStatusEffectData statusEffectData)
    {
        StatusEffectBase statusEffect = statusEffectData.InitializeStatusEffect(gameObject);

        bool hasMerged = false;

        if (MergeEffectsOfSameType)
        {
            // Todo: not efficient if there are many status effects on an object
            System.Type statusEffectType = statusEffect.GetType();
            foreach (StatusEffectBase activeStatusEffect in CurrentStatusEffects)
            {
                if (activeStatusEffect.GetType() == statusEffectType)
                {
                    activeStatusEffect.Merge(statusEffect);
                    hasMerged = true;
                }
            }
        }

        if (hasMerged == false)
        { 
            CurrentStatusEffects.Add(statusEffect);
            statusEffect.OnActivate();
        }

    }

    /// <summary>
    /// Add a list of new status effects to this component.
    /// </summary>
    /// <param name="statusEffectsData">The list of status effects to add</param>
    public void AddStatusEffect(List<ScriptableStatusEffectData> statusEffectsData)
    {
        for (int i = 0; i < statusEffectsData.Count; i++)
        {
            AddStatusEffect(statusEffectsData[i]);
        }
    }
}
