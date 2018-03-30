using UnityEngine;

public class HealthComponent : PropertyComponentBase
{
    public override void IncreaseValue(float amount)
    {
        if (amount <= 0)
            return;

        photonView.RPC("IncreaseHealthValue", PhotonTargets.MasterClient, amount);
    }

    public override void DecreaseValue(float amount)
    {
        if (amount <= 0)
            return;

        photonView.RPC("DecreaseHealthValue", PhotonTargets.MasterClient, amount);
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
    private void IncreaseHealthValue(float amount)
    {
        Value += amount;
    }

    [PunRPC]
    private void DecreaseHealthValue(float amount)
    {
        Value -= amount;
    }
}
