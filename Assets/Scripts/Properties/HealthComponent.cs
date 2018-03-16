using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : Photon.MonoBehaviour, IPunObservable
{
    public delegate void OnValueChanged(float value);
    public OnValueChanged OnValueChangedCallback;

    [SerializeField] private float MaxValue = 100;
    [SerializeField] private float MinValue = 0;

    [SerializeField] private float health;
    public float Health
    {
        get { return health; }
        set
        {
            health = Mathf.Clamp(value, MinValue, MaxValue);
            OnValueChangedCallback?.Invoke(health);
            Debug.Log(health);
        }
    }
    
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (float)stream.ReceiveNext();
        }
    }
    
    public bool IsDepleted()
    {
        if (health <= 0)
        {
            return true;
        }
        return false;
    }

}
