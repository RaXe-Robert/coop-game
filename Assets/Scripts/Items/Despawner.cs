using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawner : MonoBehaviour {

    private float timeToDespawn = 300F;

	// Use this for initialization
	void Start () {
        StartCoroutine(Despawn());	
	}	

    IEnumerator Despawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeToDespawn -= 1;

            if (timeToDespawn < 1)
            {
                Destroy(gameObject);
            }
        }
    }
}
