using System.Collections.Generic;
using UnityEngine;

public class EntitiesToDropComponent : Photon.MonoBehaviour {

    [SerializeField] private List<ScriptableEntityData> EntitiesToSpawn;
    [SerializeField] private List<int> AmountPerEntity;
    [SerializeField] private int minRadius;
    [SerializeField] private int maxRadius;	
    
    public void SpawnEntitiesOnDepleted()
    {
        for (int x = 0; x < EntitiesToSpawn.Count; x++)
        {
            for (int y = 0; y < AmountPerEntity[x]; y++)
            {
                EntityFactory.CreateWorldObject(new Vector3(Random.Range(minRadius, maxRadius) + transform.position.x, 0f, Random.Range(minRadius, maxRadius) + transform.position.z), EntitiesToSpawn[x].Id , quaternion: Quaternion.Euler(0, Random.Range(0, 180), 0));
            }
        }
    }
}
