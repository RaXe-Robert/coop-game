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

    private List<StatusEffectBase> activeStatusEffects = new List<StatusEffectBase>();
    private bool isProcessing; // State that represents the running status of the ProcessActiveEffects coroutine
    
    public delegate void StatusEffectHandler(StatusEffectBase statusEffect);
    public event StatusEffectHandler OnstatusEffectAdded;
    public StatusEffectHandler OnStatusEffectFinished;

    private void Awake()
    {
        OnStatusEffectFinished += RemoveStatusEffect;
    }

    private void OnEnable()
    {
        isProcessing = false;

        if (activeStatusEffects.Count > 0)
            StartCoroutine(ProcessActiveEffects());
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
    /// <summary>
    /// Add a new status effect to this component.
    /// </summary>
    /// <param name="statusEffects">The status effect to add.</param>
    public void AddStatusEffect(ScriptableStatusEffectData statusEffectData)
    {
        StatusEffectBase statusEffect = statusEffectData.InitializeStatusEffect(this);

        bool hasMerged = false;

        if (mergeEffectsOfSameType)
        {
            // Todo: not efficient if there are many status effects active
            System.Type statusEffectType = statusEffect.GetType();
            foreach (StatusEffectBase activeStatusEffect in activeStatusEffects)
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
            activeStatusEffects.Add(statusEffect);
            OnstatusEffectAdded?.Invoke(statusEffect);

            statusEffect.Activate();
        }

        UpdateProcessingState();
    }
    
    private void RemoveStatusEffect(StatusEffectBase statusEffect)
    {
        statusEffect.End();

        activeStatusEffects.Remove(statusEffect);

        UpdateProcessingState();
    }

    /// <summary>
    /// Starts or stops the ProcessActiveEffects coroutine based on if there are any activeStatusEffects.
    /// </summary>
    private void UpdateProcessingState()
    {
        if (activeStatusEffects.Count == 0)
            StopCoroutine(ProcessActiveEffects());
        else if (!isProcessing)
        {
            isProcessing = true;
            StartCoroutine(ProcessActiveEffects());
        }
    }

    private IEnumerator ProcessActiveEffects()
    {
        WaitForSeconds waitForInverval = new WaitForSeconds(tickInterval);

        while (true)
        {
            for (int i = 0; i < activeStatusEffects.Count; i++)
            {
                activeStatusEffects[i].Tick(tickInterval);
            }

            yield return waitForInverval;
        }
    }
}
