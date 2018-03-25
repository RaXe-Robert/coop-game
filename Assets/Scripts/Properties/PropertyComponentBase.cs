using System;
using System.Collections;

using UnityEngine;

/// <summary>
/// Base class for all properties, the main purpose of these properties are to keep track of a single Value and to be able to modify this value.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public abstract class PropertyComponentBase : Photon.MonoBehaviour, IPunObservable
{
    public delegate void OnValueChanged(float value);
    public event OnValueChanged OnValueChangedCallback;

    [Tooltip("Completely disable world notifications for this object.")]
    [SerializeField] protected bool ShowNotifications = true;
    [Tooltip("Don't show notifications for this object if it's a local object.")]
    [SerializeField] protected bool ShowNotificationsIfLocal = true;

    [SerializeField] protected float MaxValue = 100f;
    [SerializeField] protected float value;
    public float Value
    {
        get
        {
            return ValueRequestAction();
        }
        private set
        {
            float previousValue = this.value;

            this.value = Mathf.Clamp(value, 0f, MaxValue);

            if (previousValue != this.value)
            {
                OnValueChangedCallback?.Invoke(this.value);
                
                if (ShowNotifications && photonView.isMine)
                {
                    ValueChangedNotification(previousValue, this.value);
                }
            }
        }
    }

    public bool IsDepleted ()
    {
        return Value <= 0f;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(value);
        }
        else
        {
            Value = (float)stream.ReceiveNext();
        }
    }

    /// <summary>
    /// Change the value of this component positively.
    /// </summary>
    /// <param name="amount">The amount to add.</param>
    public void IncreaseValue(float amount)
    {
        if (amount < 0)
            return;
        
        Value += amount;
    }

    /// <summary>
    /// Change the value of this component negatively.
    /// </summary>
    /// <param name="amount">The amount to subtract.</param>
    public void DecreaseValue(float amount)
    {
        if (amount < 0)
            return;

        Value -= amount;
    }

    /// <summary>
    /// If an outside class calls the getter of this Value apply certain effects to this value first
    /// </summary>
    /// <returns>A modified Value.</returns>
    protected abstract float ValueRequestAction();
    /// <summary>
    /// If an outside class calls the setter of this Value perform certain actions
    /// </summary>
    /// <param name="previousValue">The value before the setter was applied.</param>
    /// <param name="currentValue">The current Value.</param>
    protected abstract void ValueChangedNotification(float previousValue, float currentValue);
}
