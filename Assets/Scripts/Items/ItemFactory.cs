using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemFactory : MonoBehaviour {

    private static ItemData[] itemLookUpTable;
    private PhotonView photonView;
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
            ((Resource)item).StackSize = stackSize;
        }
        else if (type == typeof(ArmorData))
            item = new Armor(itemData as ArmorData);
        else
            item = new Weapon(itemData as WeaponData);

        return item;
    }

    /// <summary>
    /// Creates a new item in the world
    /// </summary>
    /// <param name="item">The item to instantiate a world object of</param>
    /// <param name="position">The position for the item to spawn at</param>
    /// <param name="parent">The parent gameObject for the instantiated item</param>
    /// <returns>The newly created gameObject</returns>
    public void CreateWorldObject(Item itemData, Vector3 position, Transform parent = null)
    {
        var photonId = PhotonNetwork.AllocateViewID();
        photonView.RPC("SpawnItemOnNetwork", PhotonTargets.AllBuffered, position, photonId, itemData.Id);
    }

    [PunRPC]
    void SpawnItemOnNetwork(Vector3 position, int photonId, int itemId)
    {
        GameObject go = Resources.Load<GameObject>("Item");

        //ItemData item = CreateNewItem(itemId);
        Item item = CreateNewItem(itemId);

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
