using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNetworkTree : MonoBehaviour {

    [SerializeField] private GameObject go;

	void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < 1000; i += 10)
            {
                Vector3 position = new Vector3(Random.Range(i, -100), 0f, Random.Range(-100, i));
                PhotonNetwork.InstantiateSceneObject(go.name, position, Quaternion.Euler(0, Random.Range(0, 180), 0), 0, null);
            }
        }
    }
    
}
