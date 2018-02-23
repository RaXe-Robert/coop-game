using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour {

    public Item[] itemLookUpTable;
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Creates a new item in the world
    /// </summary>
    /// <param name="item">The item to instantiate a world object of</param>
    /// <param name="position">The position for the item to spawn at</param>
    /// <param name="parent">The parent gameObject for the instantiated item</param>
    /// <returns>The newly created gameObject</returns>
    public void CreateWorldObject(Item item, Vector3 position, Transform parent = null)
    {
        var photonId = PhotonNetwork.AllocateViewID();
        photonView.RPC("SpawnItemOnNetwork", PhotonTargets.AllBuffered, position, photonId, item.Id);
    }

    [PunRPC]
    void SpawnItemOnNetwork(Vector3 position, int photonId, int itemId)
    {
        GameObject go = Resources.Load<GameObject>("Item");

        var item = itemLookUpTable[itemId];

        go.name = item.name;

        //Get the mesh and materials from the referenced model.
        var itemMesh = item.Model.GetComponent<MeshFilter>().sharedMesh;

        //Assign the mesh and materials to the new gameObject.
        go.GetComponent<MeshRenderer>().sharedMaterials = item.Model.GetComponent<MeshRenderer>().sharedMaterials;
        go.GetComponent<MeshFilter>().sharedMesh = itemMesh;

        //Create the collider and make it convex
        var coll = go.GetComponent<MeshCollider>();
        coll.sharedMesh = itemMesh;
        coll.convex = true;

        go.GetComponent<ItemWorldObject>().item = item;

        var gameObj = Instantiate(go, position, Quaternion.identity);
        PhotonView[] nViews = gameObj.GetComponentsInChildren<PhotonView>();
        nViews[0].viewID = photonId;
    }
}
