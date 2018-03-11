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

    public Item CreateNewItem(int itemId, int stackSize = 1)
    {
        Item itemData = itemLookUpTable[itemId];

        if (itemData.GetType() == typeof(Item))
            Item item = Item.CreateInstance(itemData);
        else if (itemData.GetType() == typeof(Resource))
        {
            item = Resource.CreateResource(itemData as Resource) as Resource;
            if (item.GetType() == typeof(Resource))
                item.StackSize = 5;
        }
        else if (itemData.GetType() == typeof(Armor))
            item = Armor.CreateArmor(itemData as Armor);
        else item = Weapon.CreateWeapon(itemData as Weapon);

        return item;
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

        Item itemData = itemLookUpTable[itemId];
        Item item;

        

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
        gameObj.name = item.name;

        PhotonView[] nViews = gameObj.GetComponentsInChildren<PhotonView>();
        nViews[0].viewID = photonId;
    }
}
