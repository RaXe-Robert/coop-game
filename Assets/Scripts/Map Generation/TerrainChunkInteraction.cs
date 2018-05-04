using UnityEngine;
using System.Collections;

public class TerrainChunkInteraction : MonoBehaviour
{
    private TerrainChunk terrainChunk;
    public TerrainChunk SetTerrainChunk
    {
        set
        {
            this.terrainChunk = value;
            debugRayPosition = new Vector3(terrainChunk.SampleCenter.x, transform.position.y, terrainChunk.SampleCenter.y) * terrainChunk.MeshSettings.MeshScale;
        }
    }

    private Vector3 debugRayPosition = Vector3.zero;

    private void OnMouseDown()
    {
        Debug.Log(terrainChunk.SampleCenter);
        Debug.Log(terrainChunk.Bounds.extents);
        Ray mouseRay = PlayerNetwork.PlayerObject.GetComponent<PlayerCameraController>().CameraReference.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        RaycastHit raycastHitInfo;
        if (Physics.Raycast(mouseRay, out raycastHitInfo))
        {
            debugRayPosition = raycastHitInfo.point;

            int biomeMapLength = terrainChunk.MapData.BiomeMap.Values.GetLength(0);
            int heightMapLength = terrainChunk.MapData.HeightMap.values.GetLength(0);

            Vector3 distance = new Vector3(raycastHitInfo.point.x, 0f, raycastHitInfo.point.z) - new Vector3(transform.position.x, 0f, transform.position.z);
            
            float xPercentage = distance.x / terrainChunk.Bounds.size.x * 100f + 50f; // +50 since distance from middle is on scale of 0 to 50 or 0 to -50
            float zPercentage = distance.z / terrainChunk.Bounds.size.y * 100f + 50f; // +50 since distance from middle is on scale of 0 to 50 or 0 to -50
            zPercentage = Mathf.Abs(zPercentage - 100); // Invert

            float range = terrainChunk.Bounds.size.x / terrainChunk.MeshSettings.MeshScale; // 490 / 5 = 98 points

            int xPoint = Mathf.RoundToInt(range / 100f * xPercentage) + 1; // +1 since we don't use the outer points
            int zPoint = Mathf.RoundToInt(range / 100f * zPercentage) + 1; // +1 since we don't use the outer points

            Biome biome = terrainChunk.MapData.BiomeMap.GetBiomeFromValue(xPoint, zPoint);
            float heightValue = terrainChunk.MapData.HeightMap.values[xPoint, zPoint];

            Debug.Log($"BiomeValue: {biome.name}, HeightValue: {heightValue} : {debugRayPosition.y}");
        }
    }

    private void Update()
    {
        // Clicked position
        Debug.DrawRay(debugRayPosition, Vector3.up * 100f, new Color(255f,0f,0f));

        Debug.DrawLine(debugRayPosition + Vector3.up * 10f, transform.position + Vector3.up * 10f);
    }
}
