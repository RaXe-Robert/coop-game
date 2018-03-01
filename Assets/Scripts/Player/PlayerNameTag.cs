using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds a nametag at the players position
/// </summary>
public class PlayerNameTag : Photon.MonoBehaviour {

    private PlayerCameraController cameraController;

    [SerializeField] Vector3 positionOffset = Vector3.zero;
    [SerializeField] private float width = 50;
    [SerializeField] private float height = 5;

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

    private void Start()
    {
        cameraController = PlayerNetwork.PlayerObject?.GetComponent<PlayerCameraController>();
    }

    private void OnGUI()
    {
        if (cameraController?.CameraReference == null)
            return;
        
        Vector3 screenPos = cameraController.CameraReference.WorldToScreenPoint(transform.position + positionOffset);
        screenPos.y = Screen.height - screenPos.y;

        GUI.Label(new Rect(screenPos.x - (width / 2), screenPos.y - (height / 2), width, height), photonView.owner.NickName, guiStyle);
    }
}
