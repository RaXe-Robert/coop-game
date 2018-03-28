using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class MovementSpeedComponent : PropertyComponentBase
{
    protected override void ValueChangedNotification(float previousValue, float currentValue)
    {
        float changedAmount = currentValue - previousValue;

        WorldNotificationsManager.Instance?.ShowNotification(
            new WorldNotificationArgs(
                transform.position,
                changedAmount < 0 ? "Gotta go fast!" : "Slowed!",
                0.4f,
                changedAmount < 0 ? "red" : "green"
                ), ShowNotificationsIfLocal
        );
    }

    protected override float ValueRequestAction()
    {
        float modifiedValue = value;

        return modifiedValue;
    }
}
