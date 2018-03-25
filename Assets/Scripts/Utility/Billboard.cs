using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates the object that has this attached towards the camera
/// </summary>
public class Billboard : MonoBehaviour
{
    [SerializeField] private bool flip = true;

    private Camera cameraToFace = null;
    
    private void LateUpdate()
    {
        FaceCamera();
    }

    private void FaceCamera()
    {
        if (cameraToFace == null)
        {
            cameraToFace = PlayerNetwork.PlayerObject?.GetComponent<PlayerCameraController>()?.CameraReference;
            return;
        }

        transform.LookAt(cameraToFace.transform.position, cameraToFace.transform.up);

        if (flip)
        {
            transform.Rotate(new Vector3(0, 180, 0));
        }
    }
}
