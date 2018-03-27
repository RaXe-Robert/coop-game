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

    public void ActivateBuildMode(Buildable buildable)
    {
        ClearCurrentBuildableToBuild();

        Object buildableResource = Resources.Load($"Buildables/{buildable.Name}");
        if (buildableResource != null)
        {
            buildableToBuild = Instantiate(buildableResource, PlayerNetwork.PlayerObject.transform.position, Quaternion.identity) as GameObject;
        }
        else
        {
            Debug.LogError("BuildingController - Buildable not found");
        }
    }

    private void ClearCurrentBuildableToBuild()
    {
        if (buildableToBuild == null)
            return;

        Destroy(buildableToBuild);
    }

}
