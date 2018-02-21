using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory {
    /// <summary>
    /// Creates a new item in the world
    /// </summary>
    /// <param name="item">The item to instantiate a world object of</param>
    /// <param name="position">The position for the item to spawn at</param>
    /// <param name="parent">The parent gameObject for the instantiated item</param>
    /// <returns>The newly created gameObject</returns>
    public static GameObject CreateWorldObject(Item item, Vector3 position, Transform parent = null)
    {
        GameObject go = new GameObject()
        {
            name = item.name
        };
        go.transform.position = position;
        go.transform.SetParent(parent);

        //Get the mesh and materials from the referenced model.
        var itemMesh = item.Model.GetComponent<MeshFilter>().sharedMesh;
        var itemMaterials = item.Model.GetComponent<MeshRenderer>().sharedMaterials;

        //Assign the mesh and materials to the new gameObject.
        go.AddComponent<MeshRenderer>().sharedMaterials = itemMaterials;
        go.AddComponent<MeshFilter>().sharedMesh = itemMesh;

        //Create the collider and make it convex
        var coll = go.AddComponent<MeshCollider>();
        coll.sharedMesh = itemMesh;
        coll.convex = true;

        go.AddComponent<Rigidbody>();
        go.AddComponent<ItemWorldObject>().item = item;

        return go;
    }
}
