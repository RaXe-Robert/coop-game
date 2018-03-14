using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsToDropComponent : Photon.MonoBehaviour {
    
    [SerializeField] private List<GameObject> gameObjectToSpawn;
    [SerializeField] private List<int> gameObjectCountPerObject;
    [SerializeField] private int minRadius;
    [SerializeField] private int maxRadius;	
    
    public void SpawnObjectsOnDepleted()
    {
        for (int x = 0; x < gameObjectToSpawn.Count; x++)
        {
            for (int y = 0; y < gameObjectCountPerObject[x]; y++)
            {                
                photonView.RPC("SpawnObjectInRadius", PhotonTargets.MasterClient, gameObjectToSpawn[x].name);
            }
        }
    }

    public void SpawnObjectOnParent(GameObject objectToSpawn)
    {
        Quaternion objectToSpawnRotation = transform.rotation * objectToSpawn.transform.rotation;
        photonView.RPC("SpawnObjectOnParent", PhotonTargets.MasterClient, objectToSpawn.name, objectToSpawnRotation);
    }

    [PunRPC]
    void SpawnObjectInRadius(string go)
    {
        Vector3 position = new Vector3(Random.Range(minRadius, maxRadius) + this.gameObject.transform.position.x, 0f, Random.Range(minRadius, maxRadius) + this.gameObject.transform.position.z);
        PhotonNetwork.InstantiateSceneObject(go, position, Quaternion.Euler(0, Random.Range(0,180), 0), 0, null);
    }

    [PunRPC]
    void SpawnObjectOnParent(string go, Quaternion objectToSpawnRotation)
    {
        PhotonNetwork.InstantiateSceneObject(go, transform.position, objectToSpawnRotation, 0, null);
    }
}
