using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNetworkTree : MonoBehaviour {

    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject rock;
    public int maxTrees = 10;
    public int maxRocks = 10;

    void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            GameObject TreeCollection = new GameObject("Tree collection");
            GameObject RockCollection = new GameObject("Stone colletion");

            for (int i = 0; i < maxTrees; i++)
            {
                var newTree = PhotonNetwork.InstantiateSceneObject(tree.name, new Vector3(Random.Range(-50, 50), 0f, Random.Range(-50, 50)), Quaternion.Euler(0, Random.Range(0, 180), 0), 0, null);
                newTree.transform.SetParent(TreeCollection.transform);
            }

            for (int i = 0; i < maxTrees; i++)
            {
                var newRock = PhotonNetwork.InstantiateSceneObject(rock.name, new Vector3(Random.Range(-50, 50), 0f, Random.Range(-50, 50)), Quaternion.Euler(0, Random.Range(0, 180), 0), 0, null);
                newRock.transform.SetParent(RockCollection.transform);
            }

        }
    }
    
}
