using System;
using System.Collections;

using UnityEngine;

/// <summary>
/// Base class for all status effects data objects.
/// </summary>
public abstract class ScriptableStatusEffectData : ScriptableObject
{ 
    // The duration of the status effect
    public float Duration;

    /// <summary>
    /// Returns a certain type of status effect based on this type.
    /// </summary>
    /// <param name="gameObj">Target object for the status effect.</param>
    public abstract StatusEffectBase InitializeStatusEffect(GameObject gameObj);
}
