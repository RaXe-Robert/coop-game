using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityFactory : MonoBehaviour {

    private static ScriptableEntityData[] entityLookUpTable;
    private static PhotonView photonView;
    public EntityBase test;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        entityLookUpTable = Resources.LoadAll<ScriptableEntityData>("Entities");        
    }

    public static EntityBase CreateNewEntity(int entityId, int stackSize = 1)
    {
        var entityData = entityLookUpTable.First(x => x.Id == entityId);

        EntityBase entity = entityData.InitializeEntity();
        entity.StackSize = stackSize;
        
        return entity;
    }

    public static void CreateWorldObject(Vector3 position, int entityId, int stackSize = 1, Quaternion quaternion = new Quaternion())
    {
        var photonId = PhotonNetwork.AllocateViewID();
        photonView.RPC("SpawnItemOnNetwork", PhotonTargets.AllBuffered, position, photonId, entityId, quaternion, stackSize);
    }

    [PunRPC]
    private void SpawnItemOnNetwork(Vector3 position, int photonId, int entityId, Quaternion quaternion = new Quaternion(), int stackSize = 1)
    {
        GameObject go = Resources.Load<GameObject>("Entity");

        EntityBase entity = CreateNewEntity(entityId, stackSize);

        //Get the mesh and materials from the referenced model.
        var entityMesh = entity.Model.GetComponent<MeshFilter>().sharedMesh;

        var gameObj = Instantiate(go, position, quaternion);
        gameObj.GetComponent<EntityWorldObject>().entity = entity;
        gameObj.name = entity.Name;

        //Assign the mesh and materials to the new gameObject.
        gameObj.GetComponent<MeshRenderer>().sharedMaterials = entity.Model.GetComponent<MeshRenderer>().sharedMaterials;
        gameObj.GetComponent<MeshFilter>().sharedMesh = entityMesh;

        //Create the collider and make it convex
        var coll = gameObj.GetComponent<MeshCollider>();
        coll.sharedMesh = entityMesh;
        coll.convex = true;

        PhotonView[] nViews = gameObj.GetComponentsInChildren<PhotonView>();
        nViews[0].viewID = photonId;
    }
}
