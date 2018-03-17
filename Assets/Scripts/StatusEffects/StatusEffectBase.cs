using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all status effects
/// </summary>
public abstract class StatusEffectBase
{
    protected GameObject gameObj; // The target object for this status effect
    protected ScriptableStatusEffect statusEffect; // Contains specific data for the status effect
    public float Duration => statusEffect.Duration; // Total duration of this effect

    protected float timeRemaining;
    public float TimeRemaing { get { return timeRemaining; } }

    public bool IsFinished
    {
        get { return timeRemaining <= 0 ? true : false; }
    }

    public StatusEffectBase(ScriptableStatusEffect statusEffect, GameObject gameObj)
    {
        this.gameObj = gameObj;
        this.statusEffect = statusEffect;
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
        if (timeRemaining <= 0)
            OnEnd();
    }

    /// <summary>
    /// Gets called when this is added to a <see cref="StatusEffectComponent"/>
    /// </summary>
    public abstract void OnActivate();
    
    public abstract void OnTick(float delta);
    
    public abstract void OnEnd();

    public abstract override string ToString();
}
