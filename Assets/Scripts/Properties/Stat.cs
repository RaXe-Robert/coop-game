using UnityEngine;
using System.Collections;

public class Stat
{
    private float baseValue;
    private float currentValue;
    private PlayerStats playerStats;

    public float CurrentValue => currentValue;
    public float BaseValue
    {
        get
        {
            return baseValue;
        }

        set
        {
            currentValue -= baseValue + value;
            baseValue = value;
            playerStats.OnValueChangedCallback?.Invoke();
        }
    }

    public Stat(float baseValue, PlayerStats playerStats)
    {
        this.playerStats = playerStats;
        this.baseValue = baseValue;
        this.currentValue = baseValue;
    }

    public void AddValue(float value)
    {
        currentValue += value;
        playerStats.OnValueChangedCallback?.Invoke();
    }

    public void RemoveValue(float value)
    {
        currentValue -= value;
        playerStats.OnValueChangedCallback?.Invoke();
    }
}

