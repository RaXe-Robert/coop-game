using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.Map_Generation
{
    /// <summary>
    /// Container class for a collection of resources. Able to spawn instances or destroy them.
    /// </summary>
    public class TerrainChunkPart
    {
        public readonly Vector2 Coord;
        public readonly Vector3 WorldPosition;

        private readonly TerrainChunk terrainChunk;

        public readonly Dictionary<double, ObjectPoint> ObjectPoints = new Dictionary<double, ObjectPoint>();
        private readonly Dictionary<double, GameObject> spawnedInstances = new Dictionary<double, GameObject>();

        private bool isVisible;
        public bool Visible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    if (isVisible)
                        SpawnObjects();
                    else
                        DespawnObjects();
                }
            }
        }

        public TerrainChunkPart(Vector2 coord, Vector3 worldPosition, TerrainChunk terrainChunk)
        {
            this.Coord = coord;
            this.WorldPosition = worldPosition;
            this.terrainChunk = terrainChunk;
        }

        public void AddObjectPoint(ObjectPoint objectPoint)
        {
            // Create an unique id.
            float a = objectPoint.position.x + objectPoint.position.y;
            float b = objectPoint.position.z + objectPoint.position.y;
            double id = 0.5 * (a + b) * (a + b + 1) + b;
            
            if (!ObjectPoints.ContainsKey(id))
                ObjectPoints.Add(id, objectPoint);
            else
                Debug.LogError($"An ResourcePoint with the same id: `{id}` already exists.");
        }

        /// <summary>
        /// Returns true if the object was found and removed.
        /// </summary>
        /// <param name="id"></param>
        public bool RemoveObjectPoint(double id)
        {
            bool objectPointFound = false;

            // See if the object is spawned, if so it has to be destroyed aswell
            if (spawnedInstances.ContainsKey(id))
            {
                GameObject go = spawnedInstances[id];
                spawnedInstances.Remove(id);
                Object.Destroy(go);

                objectPointFound = true;
            }

            if (ObjectPoints.ContainsKey(id))
            {
                ObjectPoints.Remove(id);

                objectPointFound = true;
            }

            return objectPointFound;
        }

        private void SpawnObjects()
        {
            foreach (var objectPoint in ObjectPoints)
            {
                GameObject go = Object.Instantiate(TerrainGenerator.BiomeResources[objectPoint.Value.biomeId].worldResourceEntries[objectPoint.Value.worldResourcePrefabId].WorldResourcePrefab, objectPoint.Value.position, objectPoint.Value.rotation, terrainChunk.MeshObject.transform);
                go.GetComponent<WorldResource>().Setup(terrainChunk, objectPoint.Key);
                spawnedInstances.Add(objectPoint.Key, go);
            }
        }

        private void DespawnObjects()
        {
            foreach (var instance in spawnedInstances)
                Object.Destroy(instance.Value);
            spawnedInstances.Clear();
        }
    }
}
