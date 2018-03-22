using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class HealthEventArgs : EventArgs
{
    public float Damage { get; }

    public HealthEventArgs(float damage)
    {
        Damage = damage;
    }
}

public class HealthComponent : PropertyComponentBase
{
    public delegate void HealthEventHandler(object sender, HealthEventArgs eventArgs);
    public event HealthEventHandler HealthEvent;

    public void TakeDamage(float damage)
    {
        HealthEventArgs healthArgs = new HealthEventArgs(damage);

        HealthEvent(this, healthArgs);
    }
}
