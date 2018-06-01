using System;
using System.Collections;

using UnityEngine;

/// <summary>
/// Controller class for building and placing 'buildable' objects in the world.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class BuildingController : Photon.MonoBehaviour
{
    [Range(1f,15f)]
    [SerializeField] private float buildingRange = 5f;
    [Range(1f, 10f)]
    [SerializeField] private float gridSpacing = 5f;
    [SerializeField] private Color gridSnappingActiveColor;
    [SerializeField] private Color gridSnappingInactiveColor;
    [SerializeField] private Color gridOutOfRangeColor;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Material buildingModeMaterial;

    // The object that is currently selected by the player to be build.
    private GameObject buildableToBuild = null;
    private Renderer buildableToBuildRenderer = null;
    private BuildableBase buildableData = null;

    private Renderer gridRenderer;

    private void Awake()
    {
        gridRenderer = GetComponent<Renderer>();
        gridRenderer.enabled = false;
    }

    private void Update()
    {
        // TODO: Hardcoded input
        if (Input.GetMouseButtonDown(0))
        {
            FinishBuildMode();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ExitBuildMode();
        }
    }

    /// <summary>
    /// Activates the building mode, the buildable will be instantiated and follows the mouse.
    /// </summary>
    /// <param name="buildable"></param>
    public void ActivateBuildMode(BuildableBase buildable)
    {
        ExitBuildMode();
        
        GameObject buildableResource = Resources.Load<GameObject>($"Buildable");
        if (buildableResource != null)
        {
            buildableData = buildable;

            buildableToBuild = Instantiate(buildableResource, PlayerNetwork.LocalPlayer.transform.position, Quaternion.identity);
            buildableToBuild.name = buildable.Name;

            buildableToBuildRenderer = buildableToBuild.GetComponent<Renderer>();

            buildableToBuild.GetComponent<MeshRenderer>().sharedMaterials = GetReplacementMaterials(buildable.Model.GetComponent<MeshRenderer>().sharedMaterials);
            buildableToBuild.GetComponent<MeshFilter>().sharedMesh = buildable.Model.GetComponent<MeshFilter>().sharedMesh;
            
            gridRenderer.enabled = true;
            gridRenderer.sharedMaterial.SetFloat("_GridSpacing", gridSpacing);
            gridRenderer.sharedMaterial.SetFloat("_GridDistance", buildingRange);

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
        if (ConfirmBuildingConditions() == false)
            return;

        FindObjectOfType<Inventory>().RemoveItemById(buildableData.Id);
        var photonId = PhotonNetwork.AllocateViewID();
        photonView.RPC(nameof(RPC_SpawnBuildable), PhotonTargets.AllBuffered, buildableToBuild.transform.position, photonId, buildableData.Id, buildableToBuild.transform.rotation);

        ExitBuildMode();
    }

    /// <summary>
    /// When building is cancelled or stopped the buildable should be removed from the world
    /// </summary>
    private void ExitBuildMode()
    {
        if (buildableToBuild == null)
            return;
        
        gridRenderer.enabled = false;
        Destroy(buildableToBuild);
    }

    private IEnumerator FollowMouse()
    {
        PlayerCameraController playerCameraController = PlayerNetwork.LocalPlayer.GetComponent<PlayerCameraController>();
        Transform playerTransform = PlayerNetwork.LocalPlayer.transform;
        Material buildingGrid = GetComponent<Renderer>().sharedMaterial;

        while (buildableToBuild != null)
        {
            Ray ray = playerCameraController.CameraReference.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000f, layerMask.value))
            {
                Vector3 newPosition = hitInfo.point;

                if (buildableData.SnapToGrid)
                {
                    // Calculate X positon
                    float xSpacing = Mathf.Abs(newPosition.x % gridSpacing);
                    float xPosition = xSpacing >= (gridSpacing / 2) ? gridSpacing - xSpacing : xSpacing * -1;
                    
                    // Calculate Z position
                    float zSpacing = Mathf.Abs(newPosition.z % gridSpacing);
                    float zPosition = zSpacing >= (gridSpacing / 2) ? gridSpacing - zSpacing : zSpacing * -1;

                    // Store the position
                    newPosition.x += newPosition.x >= 0 ? xPosition : xPosition * -1;
                    newPosition.z += newPosition.z >= 0 ? zPosition : zPosition * -1;

                }
                // Apply the position
                buildableToBuild.transform.position = newPosition;
                
                //Update the building grid
                transform.position = newPosition;
                buildingGrid.SetVector("_Point", newPosition);

                if (ConfirmBuildingConditions())
                    gridRenderer.sharedMaterial.SetColor("_GridColor", (buildableData.SnapToGrid ? gridSnappingActiveColor : gridSnappingInactiveColor));
                else
                    gridRenderer.sharedMaterial.SetColor("_GridColor", gridOutOfRangeColor);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Replaces all given materials with the <see cref="buildingModeMaterial"/>
    /// </summary>
    private Material[] GetReplacementMaterials(Material[] materials)
    {
        Material[] replacementMaterials = new Material[materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            replacementMaterials[i] = buildingModeMaterial;
        }

        return replacementMaterials;
    }

    /// <summary>
    /// Checks if the current location of the buildableToBuild object is suitable to be build on
    /// </summary>
    /// <returns></returns>
    private bool ConfirmBuildingConditions()
    {
        if (buildableToBuild == null || buildingRange < Vector3.Distance(PlayerNetwork.LocalPlayer.transform.position, buildableToBuild.transform.position))
            return false;

        if (buildableToBuildRenderer)
        {
            Collider[] overlappingObjects = Physics.OverlapBox(buildableToBuildRenderer.bounds.center, buildableToBuildRenderer.bounds.extents);
            foreach (Collider collider in overlappingObjects)
            {
                //If the layer of the collider object is anything but the given layerMask
                if ((1 << collider.gameObject.layer & layerMask.value) <= 0)
                    return false;
            }
        }
        return true;
    }

    [PunRPC]
    private void RPC_SpawnBuildable(Vector3 position, int photonId, string itemId, Quaternion quaternion = new Quaternion())
    {
        BuildableBase buildable = ItemFactory.CreateNewItem(itemId) as BuildableBase;
        var prefab = buildable.PrefabToSpawn;

        GameObject gameObj = Instantiate(prefab, position, quaternion);
        gameObj.GetComponent<BuildableWorldObject>().enabled = true;
        gameObj.GetComponent<BuildableWorldObject>().buildable = buildable;
        if (gameObj.GetComponent<Collider>())
            gameObj.GetComponent<Collider>().enabled = true;
        else if (gameObj.GetComponentsInChildren<Collider>() != null)
        {
            foreach (Collider c in gameObj.GetComponentsInChildren<Collider>())
            {
                gameObj.GetComponentInChildren<Collider>().enabled = true;
            }
        }        
        gameObj.name = buildable.Name;

        PhotonView[] nViews = gameObj.GetComponentsInChildren<PhotonView>();
        nViews[0].viewID = photonId;
    }
}
