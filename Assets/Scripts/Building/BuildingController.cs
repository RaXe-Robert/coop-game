using System;
using System.Collections;

using UnityEngine;

/// <summary>
/// Controller class for building and placing 'buildable' objects in the world.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class BuildingController : Photon.MonoBehaviour
{
    // The object that is currently selected by the player to be build.
    private GameObject buildableToBuild = null;
    private Buildable buildableData = null;

    [SerializeField] private bool snapToGrid;
    [Range(1f, 10f)]
    [SerializeField] private float gridSpacing;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Material buildingModeMaterial;

    private void Update()
    {
        // TODO: Hardcoded input
        if (Input.GetMouseButtonDown(0))
        {
            FinishBuildMode();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            CancelBuildMode();
        }
    }

    /// <summary>
    /// Activates the building mode, the buildable will be instantiated and follows the mouse.
    /// </summary>
    /// <param name="buildable"></param>
    public void ActivateBuildMode(Buildable buildable)
    {
        CancelBuildMode();

        UnityEngine.Object buildableResource = Resources.Load($"Buildables/{buildable.Name}");
        if (buildableResource != null)
        {
            buildableData = buildable;

            buildableToBuild = Instantiate(buildableResource, PlayerNetwork.PlayerObject.transform.position, Quaternion.identity) as GameObject;
            ReplaceMaterials();

            StartCoroutine(FollowMouse());
        }
        else
        {
            Debug.LogError($"BuildingController - Buildable '{buildable.Name}' not found");
        }
    }

    /// <summary>
    /// When building is succesfull the buildable has to be placed in the world and removed from the inventory
    /// </summary>
    private void FinishBuildMode()
    {
        if (buildableToBuild == null)
            return;

        FindObjectOfType<Inventory>().RemoveItemById(buildableData.Id);
        buildableToBuild = null;
    }

    /// <summary>
    /// When building is cancelled the buildable should be removed from the world
    /// </summary>
    private void CancelBuildMode()
    {
        if (buildableToBuild == null)
            return;

        Destroy(buildableToBuild);
    }

    private IEnumerator FollowMouse()
    {
        PlayerCameraController playerCameraController = PlayerNetwork.PlayerObject.GetComponent<PlayerCameraController>();
        Material buildingGrid = GetComponent<Renderer>().sharedMaterial;

        while (buildableToBuild != null)
        {
            Ray ray = playerCameraController.CameraReference.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000f, layerMask.value))
            {
                Vector3 position = hitInfo.point;

                if (snapToGrid)
                {
                    // Calculate X positon
                    float xSpacing = Mathf.Abs(position.x % gridSpacing);
                    float xPosition = xSpacing >= (gridSpacing / 2) ? gridSpacing - xSpacing : xSpacing * -1;
                    
                    // Calculate Z position
                    float zSpacing = Mathf.Abs(position.z % gridSpacing);
                    float zPosition = zSpacing >= (gridSpacing / 2) ? gridSpacing - zSpacing : zSpacing * -1;

                    // Store the position
                    position.x += position.x >= 0 ? xPosition : xPosition * -1;
                    position.z += position.z >= 0 ? zPosition : zPosition * -1;
                }
                // Apply the position
                buildableToBuild.transform.position = position;
                transform.position = position + Vector3.up * 0.1f;
                buildingGrid.SetVector("_Point", position);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Replaces all the materials present on the <see cref="buildableToBuild"/> with the <seealso cref="buildingModeMaterial"/>
    /// </summary>
    private void ReplaceMaterials()
    {
        if (buildableToBuild == null)
            return;

        Renderer renderer = buildableToBuild.GetComponent<Renderer>();
        Material[] materials = new Material[renderer.materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = buildingModeMaterial;
        }

        renderer.materials = materials;
    }
}
