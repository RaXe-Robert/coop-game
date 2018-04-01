using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour
{
    private Transform target;

    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private float angle = 0;
    [SerializeField] private float offset = 5;
    [SerializeField] private float zoom = 5;
    [SerializeField] private bool followOnStart;

    private bool isFollowing;
    private Camera cameraReference = null;
    public Camera CameraReference => cameraReference;

    public float Angle => angle;

    private void Awake()
    {
        target = gameObject.transform;
        isFollowing = false;
    }

    private void Start()
    {
        if (followOnStart)
            StartFollowing();
    }

    private void LateUpdate()
    {
        if (isFollowing == false)
            return;

        if (target)
        {
            if (Application.isFocused)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    angle -= 0.5f;
                else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                    angle += 0.5f;
            }
            zoom = Mathf.Clamp(zoom, 5, 20);

            Vector3 cameraPos = CalculateCameraPos(angle, offset, zoom);
            Vector3 targetPos = target.position + cameraPos;
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

    private Vector3 CalculateCameraPos(float angle, float offset, float zoom)
    {
        float x = Mathf.Cos(angle) * offset;
        float z = Mathf.Sin(angle) * offset;
        float y = zoom;
        return new Vector3(x, y, z);
    }

    public void StopFollowing()
    {
        isFollowing = false;
    }
}
