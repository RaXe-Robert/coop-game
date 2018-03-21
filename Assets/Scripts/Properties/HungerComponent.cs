using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthComponent))]
public class HungerComponent : Photon.MonoBehaviour, IPunObservable
{
    public delegate void OnValueChanged(float value);
    public OnValueChanged OnValueChangedCallback;

    [SerializeField] private float MaxValue = 100f;
    [SerializeField] private float MinValue = 0f;

    [SerializeField] private float hunger;
    public float Hunger
    {
        get { return hunger; }
        set
        {
            hunger = Mathf.Clamp(value, MinValue, MaxValue);
            OnValueChangedCallback?.Invoke(hunger);
        }
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
        WaitForSeconds waitForSeconds = new WaitForSeconds(1f);

        HealthComponent cachedHealthComponent = GetComponent<HealthComponent>();

        while (true)
        {
            yield return waitForSeconds;

            if (HungerDegenerationActive == false)
            {
                continue;
            }

            if (hunger == 0)
            {
                cachedHealthComponent.Health -= 1;
            }
            else if (hunger >= 90)
            {
                cachedHealthComponent.Health += 1;

            }

            Hunger -= 1;
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (photonView.isMine)
        {
            stream.SendNext(hunger);
        }
        else
        {
            hunger = (float)stream.ReceiveNext();
        }
    }
}
