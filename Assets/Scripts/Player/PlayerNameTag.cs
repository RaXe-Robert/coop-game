using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds a nametag at the players position
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class PlayerNameTag : Photon.MonoBehaviour {

    private PlayerCameraController cameraController;

    [SerializeField] Vector3 positionOffset = Vector3.zero;
    [SerializeField] private float width = 50f;
    [SerializeField] private float height = 5f;

    private GUIStyle guiStyle;

    private void Awake()
    {
        guiStyle = new GUIStyle
        {
            alignment = TextAnchor.MiddleCenter,
            name = "PlayerNameTag",
            richText = false
        };
    }

    private void OnEnable()
    {
        if (photonView.isMine)
            enabled = false;
    }

    private void Start()
    {
        cameraController = PlayerNetwork.PlayerObject?.GetComponent<PlayerCameraController>();
        positionOffset.y = (PlayerNetwork.PlayerObject?.GetComponent<Collider>().bounds.size.y ?? 2f) * 2.5f;
    }

    private void OnGUI()
    {
        if (photonView.owner == null || cameraController?.CameraReference == null)
            return;
        
        Vector3 screenPos = cameraController.CameraReference.WorldToScreenPoint(transform.position + positionOffset);
        screenPos.y = Screen.height - screenPos.y;

        GUI.Label(new Rect(screenPos.x - (width / 2), screenPos.y - (height / 2), width, height), photonView.owner.NickName, guiStyle);
    }
}
