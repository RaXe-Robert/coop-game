using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;

namespace Assets.Scripts.Map_Generation
{
    public class MobSpawner : Photon.MonoBehaviour
    {
        public GameObject parent;
        public float spawnRate = 60f;
        public int maxMobs = 4;
        public List<GameObject> mobs;

        private int currentMobs;
        public float spawnRadius;

        private void Start()
        {
            parent = gameObject;
            StartSpawner();
        }

        public void StartSpawner()
        {
            if (PhotonNetwork.isMasterClient && mobs.Count > 0)
            {
                StartCoroutine(Spawner());
            }
        }

        private IEnumerator Spawner()
        {
            while (true)
            {
                if (currentMobs >= maxMobs)
                {
                    yield return new WaitForSeconds(spawnRate);
                    continue;
                }

                var instantiatedObjects = new List<int>();
                var mob = mobs.PickRandom();

                var localPosition = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
                var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                var instantiated = PhotonNetwork.Instantiate(mob.name,
                        parent.transform.position + localPosition,
                        rotation, 0, null);
                instantiatedObjects.Add(instantiated.GetPhotonView().instantiationId);

                instantiated.GetComponent<NPCBase>().OnNPCKilledCallback += ReduceCurrentMobs;

                currentMobs++;

                yield return new WaitForSeconds(spawnRate);
            }
        }

        private void ReduceCurrentMobs()
        {
            currentMobs--;
        }
    }
}
