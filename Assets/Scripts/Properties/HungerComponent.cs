using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthComponent))]
public class HungerComponent : PropertyComponentBase
{
    public bool HungerDegenerationActive { get; set; } = true;
    
    public void OnEnable()
    {
        if (photonView.isMine)
        {
            StartCoroutine(HungerTick());
        }
    }

    private IEnumerator HungerTick()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1f);

        HealthComponent healthComponent = GetComponent<HealthComponent>();

        while (true)
        {
            yield return waitForSeconds;

            if (HungerDegenerationActive == false)
            {
                continue;
            }

            if (value == 0)
            {
                healthComponent.DecreaseValue(1f);
            }
            else if (value >= 90)
            {
                healthComponent.IncreaseValue(1f);

            }

            DecreaseValue(1f);
        }
    }

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
