using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates the object that has this attached towards the camera
/// </summary>
public class Billboard : MonoBehaviour
{
    [SerializeField] private bool flip = false;

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

        transform.rotation = Quaternion.LookRotation(cameraToFace.transform.forward);

        if (flip)
        {
            transform.Rotate(new Vector3(0, 180, 0));
        }
    }
}
