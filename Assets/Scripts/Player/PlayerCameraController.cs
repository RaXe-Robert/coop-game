using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour
{
    private Transform target;

    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private Vector3 offset;

    [Header("Rotation")]
    [SerializeField] private float angle = 0;
    [SerializeField] private float angleRotationSpeed = 2f;

    [Header("Zoom")]
    [SerializeField] private float zoom = 5;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float zoomMinimum = 5f;
    [SerializeField] private float zoomMaximum = 50f;

    private bool isFollowing;
    private Camera cameraReference = null;
    public Camera CameraReference => cameraReference;

    public float Angle => angle;

    private void Awake()
    {
        target = gameObject.transform;
        isFollowing = false;
    }

    private void LateUpdate()
    {
        if (isFollowing == false)
            return;

        if (target)
        {
            if (Application.isFocused)
            {
                if (Input.GetKey(KeyCode.E))
                    angle -= angleRotationSpeed * Time.deltaTime;
                if (Input.GetKey(KeyCode.Q))
                    angle += angleRotationSpeed * Time.deltaTime;

                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    zoom = Mathf.Clamp(zoom - (zoomSpeed * Time.deltaTime), zoomMinimum, zoomMaximum);
                else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                    zoom = Mathf.Clamp(zoom + (zoomSpeed * Time.deltaTime), zoomMinimum, zoomMaximum);
            }
            
            Vector3 targetPos = CalculateCameraPos(offset, angle, zoom);
            cameraReference.transform.position = targetPos;
            cameraReference.transform.LookAt(target);
        }
    }

    public void StartFollowing()
    {
        if (cameraReference != null)
        {
            isFollowing = true;
        }
        else
        {
            if (cameraPrefab == true)
            {
                GameObject newCamera = Instantiate(cameraPrefab, transform.position, Quaternion.identity, gameObject.transform) as GameObject;
                cameraReference = newCamera.GetComponent<Camera>();

                isFollowing = true;
            }
            else
                Debug.LogError("[CameraController] No Camera prefab assigned");
        }
    }

    public void StopFollowing()
    {
        isFollowing = false;
    }

    private Vector3 CalculateCameraPos(Vector3 offset, float angle, float zoom)
    {
        return target.position + (Quaternion.Euler(0, Angle * Mathf.Rad2Deg, 0) * offset).normalized * zoom;
    }
}
