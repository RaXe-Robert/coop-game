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

    public void ActivateBuildMode(Buildable buildable)
    {
        CancelBuildMode();

        Object buildableResource = Resources.Load($"Buildables/{buildable.Name}");
        if (buildableResource != null)
        {
            buildableToBuild = Instantiate(buildableResource, PlayerNetwork.PlayerObject.transform.position, Quaternion.identity) as GameObject;
            buildableData = buildable;

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
        while (buildableToBuild != null)
        {
            Ray ray = playerCameraController.CameraReference.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000f, layerMask.value))
            {
                Vector3 position = hitInfo.point;

                if (snapToGrid)
                {
                    float xSpacing = Mathf.Abs(position.x % gridSpacing);
                    float zSpacing = Mathf.Abs(position.z % gridSpacing);

                    float xOffset = (xSpacing > gridSpacing / 2) ? (gridSpacing - xSpacing) : xSpacing;
                    float zOffset = (zSpacing > gridSpacing / 2) ? (gridSpacing - zSpacing) : zSpacing;

                    if (xSpacing > gridSpacing / 2)
                    {
                        if (position.x >= 0)
                            position.x = position.x + xOffset;
                        else
                            position.x = position.x - xOffset;
                    }
                    else
                    {
                        if (position.x < 0)
                            position.x = position.x + xOffset;
                        else
                            position.x = position.x - xOffset;
                    }

                    if (zSpacing > gridSpacing / 2)
                    {
                        if (position.z >= 0)
                            position.z = position.z + zOffset;
                        else
                            position.z = position.z - zOffset;
                    }
                    else
                    {
                        if (position.z < 0)
                            position.z = position.z + zOffset;
                        else
                            position.z = position.z - zOffset;
                    }

                    Debug.Log($"From: {hitInfo.point.x} | Remainder: {xSpacing} | To: {position.x}");
                    Debug.Log($"From: {hitInfo.point.z} | Remainder: {zSpacing} | To: {position.z}");
                }
                buildableToBuild.transform.position = position;
            }
            yield return null;
        }
    }
}
