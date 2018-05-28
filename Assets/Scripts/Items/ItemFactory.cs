using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemFactory : MonoBehaviour {

    private static Dictionary<string, ScriptableItemData> itemLookupTable;
    private static PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        itemLookupTable = (Resources.LoadAll<ScriptableItemData>("Items"))?.ToDictionary(x => x.Id, x => x) ?? new Dictionary<string, ScriptableItemData>();        
    }

    public static Item CreateNewItem(string itemId, int stackSize = 1)
    {
        if (itemLookupTable.ContainsKey(itemId))
        {
            var itemData = itemLookupTable[itemId];

            Item item = itemData.InitializeItem();
            item.StackSize = stackSize;

            return item;
        }
        else
        {
            Debug.LogError($"Specificied ItemId '{itemId}' does not exist!");
            return null;
        }
    }

    public static void CreateWorldObject(Vector3 position, string itemId, int stackSize = 1, Quaternion quaternion = new Quaternion())
    {
        var photonId = PhotonNetwork.AllocateViewID();
        photonView.RPC("SpawnItemOnNetwork", PhotonTargets.AllBuffered, position, photonId, itemId, quaternion, stackSize);
    }

    [PunRPC]
    private void SpawnItemOnNetwork(Vector3 position, int photonId, string itemId, Quaternion quaternion = new Quaternion(), int stackSize = 1)
    {
        GameObject go = Resources.Load<GameObject>("Item");

        Item item = CreateNewItem(itemId, stackSize);

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
