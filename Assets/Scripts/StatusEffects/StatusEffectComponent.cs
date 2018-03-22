using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Component that is responsible for all status effects on a certain object.
/// </summary>
public class StatusEffectComponent : MonoBehaviour
{
    [SerializeField] private bool mergeEffectsOfSameType = true;
    [Range(0.1f, 1f)]
    [SerializeField] private float tickInterval = 1f;
    public List<StatusEffectBase> CurrentStatusEffects = new List<StatusEffectBase>();
    
    public delegate void StatusEffectAdded(StatusEffectBase statusEffect);
    public StatusEffectAdded OnstatusEffectAdded;

    private void OnEnable()
    {
        StartCoroutine(ProcessActiveEffects());
    }

    private IEnumerator ProcessActiveEffects()
    {
        WaitForSeconds waitForInverval = new WaitForSeconds(tickInterval);

        while (true)
        {
            foreach (StatusEffectBase statusEffect in CurrentStatusEffects.ToArray())
            {
                statusEffect.Tick(tickInterval);
                if (statusEffect.IsFinished)
                {
                    CurrentStatusEffects.Remove(statusEffect);
                }
            }

            yield return waitForInverval;
        }
    }
    
    /// <summary>
    /// Add a new status effect to this component.
    /// </summary>
    /// <param name="statusEffects">The status effect to add.</param>
    public void AddStatusEffect(ScriptableStatusEffectData statusEffectData)
    {
        StatusEffectBase statusEffect = statusEffectData.InitializeStatusEffect(gameObject);

        bool hasMerged = false;

        if (mergeEffectsOfSameType)
        {
            // Todo: not efficient if there are many status effects active
            System.Type statusEffectType = statusEffect.GetType();
            foreach (StatusEffectBase activeStatusEffect in CurrentStatusEffects)
            {
                if (activeStatusEffect.GetType() == statusEffectType)
                {
                    activeStatusEffect.Merge(statusEffect);
                    hasMerged = true;
                    break;
                }
            }
        }

        if (hasMerged == false)
        { 
            CurrentStatusEffects.Add(statusEffect);
            OnstatusEffectAdded?.Invoke(statusEffect);

            statusEffect.Activate();
        }
    }

    /// <summary>
    /// Add a list of new status effects to this component.
    /// </summary>
    /// <param name="statusEffectsData">The list of status effects to add.</param>
    public void AddStatusEffect(List<ScriptableStatusEffectData> statusEffectsData)
    {
        for (int i = 0; i < statusEffectsData.Count; i++)
        {
            AddStatusEffect(statusEffectsData[i]);
        }
    }
}
