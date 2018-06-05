using System.Collections.Generic;
using Assets.Scripts.Map_Generation;
using UnityEngine;

public class ItemsToDropComponent : MonoBehaviour {

    [SerializeField] private List<ItemToSpawnEntry> itemsToSpawn;
    [SerializeField] private int spawnRadius;

    public void SpawnItemsOnDepleted()
    {
        for (int x = 0; x < itemsToSpawn.Count; x++)
        {
            for (int y = 0; y < itemsToSpawn[x].Amount; y++)
            {
                Vector3 position = new Vector3(Random.Range(-spawnRadius, spawnRadius) + transform.position.x, 200f, Random.Range(-spawnRadius, spawnRadius) + transform.position.z);
                
                RaycastHit raycastHitInfo;
                if (Physics.Raycast(new Ray(position, Vector3.down), out raycastHitInfo, Mathf.Infinity, TerrainGenerator.LayerMask))
                    position = raycastHitInfo.point;

                ItemFactory.CreateWorldObject(position, itemsToSpawn[x].Item.Id , quaternion: Quaternion.Euler(0, Random.Range(0, 180), 0));
            }
        }
    }

    [System.Serializable]
    public class ItemToSpawnEntry
    {
        [SerializeField] private ScriptableItemData item;
        public ScriptableItemData Item => item;

        [SerializeField] private int amount;
        public int Amount => amount;
    }
}
