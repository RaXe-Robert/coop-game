using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsToDropComponent : MonoBehaviour {
    
    [SerializeField] private List<GameObject> gameObjectToSpawn;
    [SerializeField] private List<int> gameObjectCountPerObject;
    [SerializeField] private int minRadius;
    [SerializeField] private int maxRadius;	
    
    public void SpawnMultipleObjects()
    {
        PhotonView photonView = PhotonView.Get(this);
        for (int x = 0; x < gameObjectToSpawn.Count; x++)
        {
            for (int y = 0; y < gameObjectCountPerObject[x]; y++)
            {                
                photonView.RPC("SpawnObject", PhotonTargets.MasterClient, gameObjectToSpawn[x].name);
            }
        }
    }

    [PunRPC]
    void SpawnObject(string go)
    {
        Vector3 position = new Vector3(Random.Range(minRadius, maxRadius) + this.gameObject.transform.position.x, 0.5f, Random.Range(minRadius, maxRadius) + this.gameObject.transform.position.z);
        PhotonNetwork.InstantiateSceneObject(go, position, Quaternion.Euler(90, Random.Range(0,180), 0), 0, null);
    }    
}
