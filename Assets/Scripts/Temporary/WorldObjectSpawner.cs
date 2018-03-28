using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectSpawner : Photon.MonoBehaviour {

    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject rock;
    [SerializeField] private GameObject carl;
    [SerializeField] private GameObject carlScared;
    [SerializeField] private GameObject fox;

    private GameObject worldResources;

    public int maxTrees = 10;
    public int maxRocks = 10;
    public int maxNPCs = 15;

    private void Awake()
    {
        worldResources = new GameObject("Spawned World Resources");
    }

    private void Start() {

        if (PhotonNetwork.isMasterClient)
        {
            List<int> instantiatedObjects = new List<int>();

            for (int i = 0; i < maxTrees; i++)
            {
                 instantiatedObjects.Add(PhotonNetwork.InstantiateSceneObject(tree.name, new Vector3(Random.Range(-50, 50), 0f, Random.Range(-50, 50)), Quaternion.Euler(0, Random.Range(0, 180), 0), 0, null).GetPhotonView().instantiationId);
            }

            for (int i = 0; i < maxTrees; i++)
            {
                instantiatedObjects.Add(PhotonNetwork.InstantiateSceneObject(rock.name, new Vector3(Random.Range(-50, 50), 0f, Random.Range(-50, 50)), Quaternion.Euler(0, Random.Range(0, 180), 0), 0, null).GetPhotonView().instantiationId);
            }

            for (int i = 0; i < maxNPCs / 3; i++)
            {
                instantiatedObjects.Add(PhotonNetwork.InstantiateSceneObject(carl.name, new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f)), new Quaternion(), 0, null).GetPhotonView().instantiationId);
                instantiatedObjects.Add(PhotonNetwork.InstantiateSceneObject(carlScared.name, new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f)), new Quaternion(), 0, null).GetPhotonView().instantiationId);
                instantiatedObjects.Add(PhotonNetwork.InstantiateSceneObject(fox.name, new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f)), new Quaternion(), 0, null).GetPhotonView().instantiationId);
            }

            photonView.RPC("SetParents", PhotonTargets.AllBufferedViaServer, instantiatedObjects.ToArray());
        }
    }

    [PunRPC]
    private void SetParents(int[] ids)
    {
        foreach (var id in ids)
        {
            PhotonView.Find(id)?.transform?.SetParent(worldResources.transform);
        }
    }
}

    

