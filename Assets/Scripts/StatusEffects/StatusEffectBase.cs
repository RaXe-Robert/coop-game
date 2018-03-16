using UnityEngine;
using System.Collections;

public abstract class StatusEffectBase
{
    protected GameObject gameObj;
    protected ScriptableStatusEffect statusEffect;
    public float Duration => statusEffect.Duration;

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

    public void Tick(float delta)
    {
        OnTick(delta);

        timeRemaining -= delta;
        if (timeRemaining <= 0)
            OnEnd();
    }

    public abstract void OnActivate();

    public abstract void OnTick(float delta);

    public abstract void OnEnd();
}
