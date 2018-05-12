using UnityEngine;

public class HealthComponent : PropertyComponentBase
{
    public delegate void OnHealthDepleted();
    public OnHealthDepleted OnDepletedCallback;

    public override void IncreaseValue(float amount)
    {
        if (amount <= 0 || value <= 0)
            return;

        photonView.RPC("IncreaseHealthValue", PhotonTargets.AllBuffered, amount);
    }

    public override void DecreaseValue(float amount)
    {
        if (amount <= 0 || value <= 0)
            return;

        photonView.RPC("DecreaseHealthValue", PhotonTargets.AllBuffered, amount);
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
        if (Value <= 0)
            OnDepletedCallback?.Invoke();
    }
}
