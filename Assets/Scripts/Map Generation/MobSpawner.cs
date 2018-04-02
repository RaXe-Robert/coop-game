using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;

public class MobSpawner : Photon.MonoBehaviour
{
    public GameObject parent;
    public GameObject mobRoot;
    public float spawnRate = 60f;
    public int maxMobs = 7;
    public List<GameObject> mobs;

    private float tileExtent;

    // Use this for initialization
    public void StartSpawner()
    {
        if (PhotonNetwork.isMasterClient && mobs.Count > 0)
        {
            tileExtent = parent.GetComponent<Renderer>().bounds.extents.x;
            StartCoroutine(Spawner());
        }
    }

    private IEnumerator Spawner()
    {
        while(true)
        {
            Debug.Log(mobRoot.transform.childCount);
            if (mobRoot.transform.childCount >= maxMobs)
            {
                yield return new WaitForSeconds(spawnRate);
                continue;
            }

            var instantiatedObjects = new List<int>();
            var mob = mobs.PickRandom();

            var localPosition = new Vector3(Random.Range(-tileExtent, tileExtent), 0, Random.Range(-tileExtent, tileExtent));
            var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            var instantiated = PhotonNetwork.Instantiate(mob.name,
                    parent.transform.position + localPosition,
                    rotation, 0, null);
            instantiated.transform.SetParent(mobRoot.transform);
            instantiatedObjects.Add(instantiated.GetPhotonView().instantiationId);
            
            photonView.RPC("SetParents", PhotonTargets.AllBufferedViaServer, instantiatedObjects.ToArray());
            yield return new WaitForSeconds(spawnRate);
        }
    }

    [PunRPC]
    private void SetParents(int[] ids)
    {
        foreach (var id in ids)
            PhotonView.Find(id)?.transform?.SetParent(mobRoot.transform);
    }
}
