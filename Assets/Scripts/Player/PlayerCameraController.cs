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

    private void Update()
    {
        if (isFollowing == false)
            return;

        if (InputManager.Instance.GetButton("Camera rotation"))
        {
            angle += InputManager.Instance.GetAxisRaw("Mouse X") * angleRotationSpeed * 2 * Time.deltaTime;
        }

        if (target && Application.isFocused)
        {
            if (InputManager.Instance.GetButton("Left camera rotation"))
            {
                angle -= angleRotationSpeed * Time.deltaTime;
            }
            if (InputManager.Instance.GetButton("Right camera rotation"))
            {
                angle += angleRotationSpeed * Time.deltaTime;
            }
            zoom = Mathf.Clamp(zoom - (InputManager.Instance.GetAxis("Mouse ScrollWheel") * zoomSpeed), zoomMinimum, zoomMaximum);
        }
    }

    private void LateUpdate()
    {
        if (isFollowing == false)
            return;

        cameraReference.transform.position = CalculateCameraPos();
        cameraReference.transform.LookAt(target);
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
                GameObject newCamera = Instantiate(cameraPrefab, transform.position, Quaternion.identity, transform) as GameObject;
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

    private Vector3 CalculateCameraPos()
    {
        return target.position + (Quaternion.Euler(0, Angle * Mathf.Rad2Deg, 0) * offset).normalized * zoom;
    }
}
