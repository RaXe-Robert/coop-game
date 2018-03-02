using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNetworkTree : MonoBehaviour {

    [SerializeField] private GameObject go;


	// Use this for initialization
	void Start () {
        for(int i = 0; i < 1000; i+=10)
        {
            Vector3 position;
            position.x = Random.Range(i,-100);
            position.z = Random.Range(-100, i);
            position.y = 0f;
            PhotonNetwork.Instantiate(go.name, position, Quaternion.Euler(0, Random.Range(0, 180),0), 0);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
