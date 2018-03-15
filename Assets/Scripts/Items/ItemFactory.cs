using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemFactory : MonoBehaviour {

    private static ItemData[] itemLookUpTable;
    private static PhotonView photonView;
    public Item test;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        itemLookUpTable = Resources.LoadAll<ItemData>("Items");
    }

    public static Item CreateNewItem(int itemId, int stackSize = 1)
    {
        var itemData = itemLookUpTable.First(x => x.Id == itemId);
        var type = itemData.GetType();
        Item item;

        if(type == typeof(ItemData))
            item = new Item(itemData);
        else if (type == typeof(ResourceData))
        {
            item = new Resource(itemData as ResourceData);
            ((Resource)item).Amount = stackSize;
        }
        else if (type == typeof(ArmorData))
            item = new Armor(itemData as ArmorData);
        else
            item = new Weapon(itemData as WeaponData);

        return item;
    }

    public static void CreateWorldObject(Vector3 position, int itemId, int stackSize = 1)
    {
        var photonId = PhotonNetwork.AllocateViewID();
        photonView.RPC("SpawnItemOnNetwork", PhotonTargets.AllBuffered, position, photonId, itemId, stackSize);
    }

    [PunRPC]
    private void SpawnItemOnNetwork(Vector3 position, int photonId, int itemId, int stackSize = 1)
    {
        GameObject go = Resources.Load<GameObject>("Item");

        Item item = CreateNewItem(itemId, stackSize);

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
        gameObj.name = item.Name;

        PhotonView[] nViews = gameObj.GetComponentsInChildren<PhotonView>();
        nViews[0].viewID = photonId;
    }
}
