using System;
using System.Collections;

using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public abstract class PropertyComponentBase : Photon.MonoBehaviour, IPunObservable
{
    public delegate void OnValueChanged(float value);
    public event OnValueChanged OnValueChangedCallback;

    [SerializeField] protected float MaxValue = 100f;

    [SerializeField] protected float value;
    public float Value
    {
        get { return value; }
        private set
        {
            this.value = Mathf.Clamp(value, 0f, MaxValue);
            OnValueChangedCallback?.Invoke(this.value);
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
            value = (float)stream.ReceiveNext();
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
}
