using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds a nametag at the players position.
/// </summary>
/// <remarks>Will instantiate an object that is a child of the object that this script is attached to.</remarks>
[RequireComponent(typeof(PhotonView))]
public class PlayerNameTag : Photon.MonoBehaviour {

    [SerializeField] private float distanceAboveObject;
    private float objectHeight;

    private PlayerCameraController cameraController;
    private GameObject nameTagObject;
    
    private void OnEnable()
    {
        if (photonView.isMine)
        {
            enabled = false;
            return;
        }

        CreateNameTag();

        cameraController = PlayerNetwork.PlayerObject?.GetComponent<PlayerCameraController>();
        objectHeight = gameObject?.GetComponent<Collider>()?.bounds.size.y * 2 ?? 0f;
    }

    private void OnDisable()
    {
        Destroy(nameTagObject);
    }

    private void LateUpdate()
    {
        Camera camera = cameraController?.CameraReference;
        if (camera?.transform == null)
            return;

        nameTagObject.transform.localPosition = Vector3.Lerp(nameTagObject.transform.localPosition, new Vector3(0f, objectHeight + distanceAboveObject, 0f), 1f);
        nameTagObject.transform.rotation = Quaternion.LookRotation(camera.transform.forward);
    }

    private void CreateNameTag()
    {
        nameTagObject = new GameObject();
        nameTagObject.name = "PlayerNameTag";
        nameTagObject.transform.SetParent(transform);

        TextMesh nameTagTextMesh = nameTagObject.AddComponent<TextMesh>();
        nameTagTextMesh.text = photonView.owner?.NickName ?? "Unassigned";
        nameTagTextMesh.fontSize = 38;
        nameTagTextMesh.anchor = TextAnchor.MiddleCenter;
        nameTagTextMesh.alignment = TextAlignment.Center;
        nameTagTextMesh.characterSize = 0.1f;
    }
}
