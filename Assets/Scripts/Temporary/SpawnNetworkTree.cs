using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNetworkTree : Photon.MonoBehaviour {

    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject rock;
    public int maxTrees = 10;
    public int maxRocks = 10;

    void Start() {
        GameObject TreeCollection = new GameObject("Spawned Trees");        
        GameObject RockCollection = new GameObject("Spawned Stones");

        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < maxTrees; i++)
            {
                var newTree = PhotonNetwork.InstantiateSceneObject(tree.name, new Vector3(Random.Range(-50, 50), 0f, Random.Range(-50, 50)), Quaternion.Euler(0, Random.Range(0, 180), 0), 0, null);
                photonView.RPC("SetTreeParent", PhotonTargets.AllBufferedViaServer, newTree.GetPhotonView().instantiationId);
            }

            for (int i = 0; i < maxTrees; i++)
            {
                var newRock = PhotonNetwork.InstantiateSceneObject(rock.name, new Vector3(Random.Range(-50, 50), 0f, Random.Range(-50, 50)), Quaternion.Euler(0, Random.Range(0, 180), 0), 0, null);
                photonView.RPC("SetStoneParent", PhotonTargets.AllBufferedViaServer, newRock.GetPhotonView().instantiationId);
            }
        }
    }

    [PunRPC]
    void SetTreeParent(int id)
    {
        PhotonView.Find(id).gameObject.transform.SetParent(GameObject.Find("Spawned Trees").transform);
    }

    [PunRPC]
    void SetStoneParent(int id)
    {
        PhotonView.Find(id).gameObject.transform.SetParent(GameObject.Find("Spawned Stones").transform);
    }
}

    

