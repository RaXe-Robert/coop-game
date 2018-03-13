using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsToDropComponent : MonoBehaviour {
    
    [SerializeField] private List<GameObject> gameObjectToSpawn;
    [SerializeField] private List<int> gameObjectCountPerObject;
    [SerializeField] private int minRadius;
    [SerializeField] private int maxRadius;	
    
    public void SpawnObjectsOnDepleted()
    {
        PhotonView photonView = PhotonView.Get(this);
        for (int x = 0; x < gameObjectToSpawn.Count; x++)
        {
            for (int y = 0; y < gameObjectCountPerObject[x]; y++)
            {                
                photonView.RPC("SpawnObjectInRadius", PhotonTargets.MasterClient, gameObjectToSpawn[x].name);
            }
        }
    }

    public void SpawnObjectOnParent(GameObject gameObject)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("SpawnObjectOnParent", PhotonTargets.MasterClient, gameObject.name);
    }

    [PunRPC]
    void SpawnObjectInRadius(string go)
    {
        Vector3 position = new Vector3(Random.Range(minRadius, maxRadius) + this.gameObject.transform.position.x, 0f, Random.Range(minRadius, maxRadius) + this.gameObject.transform.position.z);
        PhotonNetwork.InstantiateSceneObject(go, position, Quaternion.Euler(0, Random.Range(0,180), 0), 0, null);
    }

    [PunRPC]
    void SpawnObjectOnParent(string go)
    {
        Vector3 position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        PhotonNetwork.InstantiateSceneObject(go, position, Quaternion.Euler(this.gameObject.transform.rotation.x, this.gameObject.transform.rotation.y, this.gameObject.transform.rotation.z), 0, null);
    }
}
