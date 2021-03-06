﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all status effects
/// </summary>
public abstract class StatusEffectBase
{
    protected StatusEffectComponent statusEffectComponent; // The target object for this status effect

    protected ScriptableStatusEffectData statusEffectData; // Contains specific data for the status effect
    public ScriptableStatusEffectData StatusEffectData { get { return statusEffectData; } }
    public float Duration => statusEffectData.Duration; // Total duration of this effect

    protected float timeRemaining;
    public float TimeRemaining { get { return timeRemaining; } }

    public bool IsFinished
    {
        get { return timeRemaining <= 0 ? true : false; }
    }

    public delegate void StatusEffectTick();
    public StatusEffectTick OnStatusEffectTick;

    public StatusEffectBase(ScriptableStatusEffectData statusEffect, StatusEffectComponent statusEffectComponent)
    {
        this.statusEffectComponent = statusEffectComponent;
        this.statusEffectData = statusEffect;
        this.timeRemaining = statusEffect.Duration;
    }

    /// <summary>
    /// Called by <see cref="StatusEffectComponent"/>. 
    /// </summary>
    /// <param name="delta">Frame deltatime</param>
    public void Tick(float delta)
    {
        OnTick(delta);
        timeRemaining -= delta;

        OnStatusEffectTick?.Invoke();

        if (IsFinished)
            statusEffectComponent.OnStatusEffectFinished?.Invoke(this);
    }

    public void Merge(StatusEffectBase statusEffectToMerge)
    {
        timeRemaining += statusEffectToMerge.TimeRemaining;
    }

    /// <summary>
    /// Gets called when this is added to a <see cref="StatusEffectComponent"/>
    /// </summary>
    public abstract void Activate();
    /// <summary>
    /// Gets called when this is removed from a <see cref="StatusEffectComponent"/>
    /// </summary>
    public abstract void End();

    protected abstract void OnTick(float delta);
    
    public abstract override string ToString();
}
