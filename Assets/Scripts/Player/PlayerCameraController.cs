using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour
{
    private Transform target;

    [SerializeField] private GameObject cameraPrefab;

    [SerializeField] private Vector3 centerOffset;
    [SerializeField] private bool followOnStart;

    private bool isFollowing;
    private Camera cameraReference = null;
    public Camera CameraReference { get { return this.cameraReference; } }

    private void Awake()
    {
        target = gameObject.transform;
        isFollowing = false;
    }

    private void Start()
    {
        if (followOnStart)
        {
            StartFollowing();
        }
    }

    private void LateUpdate()
    {
        if (isFollowing == false)
            return;

        if (target)
        {
            Vector3 targetPos = new Vector3(target.position.x + centerOffset.x, target.position.y + centerOffset.y, target.position.z + centerOffset.z);

            //Debug feature for animations
            if ( Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (centerOffset.y > 2 && centerOffset.y < 11)
                {
                    centerOffset.y -= 0.5f;
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (centerOffset.y > 1 && centerOffset.y < 10)
                {
                    centerOffset.y += 0.5f;
                }
            }

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
}
