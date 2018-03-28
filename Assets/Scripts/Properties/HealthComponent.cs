using UnityEngine;

public class HealthComponent : PropertyComponentBase
{
    protected override void ValueChangedNotification(float previousValue, float currentValue)
    {        
        float changedAmount = currentValue - previousValue;

        WorldNotificationsManager.Instance?.ShowNotification(
            new WorldNotificationArgs(
                transform.position,
                Mathf.Abs(changedAmount).ToString("F0"),
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
