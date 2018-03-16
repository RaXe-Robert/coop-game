using System;
using System.Collections;

using UnityEngine;

public abstract class ScriptableStatusEffect : ScriptableObject
{
    public float Duration;

    public abstract StatusEffectBase InitializeStatusEffect(GameObject gameObj);
}
