﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "StatusEffects/HealthStatusEffect")]
public class HealthStatusEffectData : ScriptableStatusEffect
{
    [Tooltip("Total health modification, can be positive or negative")]
    [SerializeField] private float healthIncrease;
    public float HealthIncrease { get { return healthIncrease; } }

    public override StatusEffectBase InitializeStatusEffect(GameObject gameObj)
    {
        return new HealthStatusEffect(this, gameObj);
    }
}
