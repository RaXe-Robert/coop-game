using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsToDropComponent : Photon.MonoBehaviour {

    [SerializeField] private List<ItemData> ItemsToSpawn;
    [SerializeField] private List<int> ItemCountPerItem;
    [SerializeField] private int minRadius;
    [SerializeField] private int maxRadius;	
    
    public void SpawnItemsOnDepleted()
    {
        for (int x = 0; x < ItemsToSpawn.Count; x++)
        {
            for (int y = 0; y < ItemCountPerItem[x]; y++)
            {
                ItemFactory.CreateWorldObject(new Vector3(Random.Range(minRadius, maxRadius) + transform.position.x, 0f, Random.Range(minRadius, maxRadius) + transform.position.z), ItemsToSpawn[x].Id , quaternion: Quaternion.Euler(0, Random.Range(0, 180), 0));
            }
        }
    }

    public void SpawnObjectOnParent(GameObject objectToSpawn)
    {
        Quaternion objectToSpawnRotation = transform.rotation * objectToSpawn.transform.rotation;
        photonView.RPC("SpawnObjectOnParent", PhotonTargets.MasterClient, objectToSpawn.name, objectToSpawnRotation);
    }

    [PunRPC]
    void SpawnObjectOnParent(string go, Quaternion objectToSpawnRotation)
    {
        PhotonNetwork.InstantiateSceneObject(go, transform.position, objectToSpawnRotation, 0, null);
    }
}
