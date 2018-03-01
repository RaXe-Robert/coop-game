using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        cameraController = FindObjectOfType<PlayerNetwork>().PlayerObject?.GetComponent<PlayerCameraController>();
    }

    private void OnGUI()
    {
        if (cameraController?.CameraReference == null)
            return;
        
        Vector3 screenPos = cameraController.CameraReference.WorldToScreenPoint(transform.position - positionOffset);
        GUI.Label(new Rect(screenPos.x - (width / 2), screenPos.y - (height / 2), width, height), photonView.owner.NickName, guiStyle);
    }
}
