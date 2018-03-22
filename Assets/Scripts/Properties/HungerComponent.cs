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
}
