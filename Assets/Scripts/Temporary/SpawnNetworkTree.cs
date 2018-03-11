using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNetworkTree : MonoBehaviour {

    [SerializeField] private GameObject go;

	void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            for (int i = -100; i < 100; i += 20)
            {
                for (int z = 100; z > -100; z -= 20)
                {
                    Vector3 position = new Vector3(Random.Range(i, z), 0f, Random.Range(i, z));
                    PhotonNetwork.InstantiateSceneObject(go.name, position, Quaternion.Euler(0, Random.Range(0, 180), 0), 0, null);
                }
            }
        }
    }
    
}
