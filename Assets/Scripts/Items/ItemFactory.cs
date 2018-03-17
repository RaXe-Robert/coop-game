using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemFactory : MonoBehaviour {

    private static ScriptableItemData[] itemLookUpTable;
    private static PhotonView photonView;
    public ItemBase test;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        itemLookUpTable = Resources.LoadAll<ScriptableItemData>("Items");
    }

    public static ItemBase CreateNewItem(int itemId, int stackSize = 1)
    {
        var itemData = itemLookUpTable.First(x => x.Id == itemId);

        ItemBase item = itemData.InitializeItem();

        if (item is Resource)
        {
            ((Resource)item).Amount = stackSize;
        }
        
        return item;
    }

    public static void CreateWorldObject(Vector3 position, int itemId, int stackSize = 1, Quaternion quaternion = new Quaternion())
    {
        var photonId = PhotonNetwork.AllocateViewID();
        photonView.RPC("SpawnItemOnNetwork", PhotonTargets.AllBuffered, position, photonId, itemId, quaternion, stackSize);
    }

    [PunRPC]
    private void SpawnItemOnNetwork(Vector3 position, int photonId, int itemId, Quaternion quaternion = new Quaternion(), int stackSize = 1)
    {
        GameObject go = Resources.Load<GameObject>("Item");

        ItemBase item = CreateNewItem(itemId, stackSize);

        //Get the mesh and materials from the referenced model.
        var itemMesh = item.Model.GetComponent<MeshFilter>().sharedMesh;

        var gameObj = Instantiate(go, position, quaternion);
        gameObj.GetComponent<ItemWorldObject>().item = item;
        gameObj.name = item.Name;

        //Assign the mesh and materials to the new gameObject.
        gameObj.GetComponent<MeshRenderer>().sharedMaterials = item.Model.GetComponent<MeshRenderer>().sharedMaterials;
        gameObj.GetComponent<MeshFilter>().sharedMesh = itemMesh;

        //Create the collider and make it convex
        var coll = gameObj.GetComponent<MeshCollider>();
        coll.sharedMesh = itemMesh;
        coll.convex = true;

        PhotonView[] nViews = gameObj.GetComponentsInChildren<PhotonView>();
        nViews[0].viewID = photonId;
    }
}
