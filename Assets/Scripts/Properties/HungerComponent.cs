using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthComponent))]
public class HungerComponent : PropertyComponentBase
{
    public override void IncreaseValue(float amount)
    {
        if (amount <= 0)
            return;

        photonView.RPC("IncreaseHungerValue", PhotonTargets.MasterClient, amount);
    }

    public override void DecreaseValue(float amount)
    {
        if (amount <= 0)
            return;

        photonView.RPC("DecreaseHungerValue", PhotonTargets.MasterClient, amount);
    }

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
        WaitForSeconds waitForSeconds = new WaitForSeconds(3f);

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

    [PunRPC]
    private void IncreaseHungerValue(float amount)
    {
        Value += amount;
    }

    [PunRPC]
    private void DecreaseHungerValue(float amount)
    {
        Value -= amount;
    }
}
