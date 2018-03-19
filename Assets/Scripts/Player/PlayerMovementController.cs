﻿using UnityEngine;
using System.Collections;

public class PlayerMovementController : Photon.MonoBehaviour
{
    Rigidbody rigidbodyComponent;
    Animator animator;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float mouseDeadZoneFromPlayer;

    private PlayerCameraController cameraController = null;

    private MeshCollider raycastPlaneCollider = null;

    private void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraController = GetComponent<PlayerCameraController>();

        CreateRaycastPlane();
    }

    // Use this for initialization
    private void Start()
    {
        if (photonView.isMine)
        {
            if (cameraController != null)
            {
                cameraController.StartFollowing();
            }
            else
            {
                Debug.LogError("Missing CameraController Component on playerPrefab.");
            }
        }

    }

    private void Update()
    {
        if (!photonView.isMine)
            return;

        RotatePlayer();
    }
    
    private void FixedUpdate()
    {
        if (!photonView.isMine)
            return;

        MovePlayer();

    }

    private void RotatePlayer()
    {
        if (cameraController.CameraReference == null)
            return;

        Ray ray = cameraController.CameraReference.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (raycastPlaneCollider.Raycast(ray, out hit, 500f))
        {
            Vector3 lookPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            if (Vector3.Distance(transform.position, lookPos) > mouseDeadZoneFromPlayer)
            {
                transform.LookAt(lookPos);
            }
        }
        
    }

    /// <summary>
    /// Moves the player based on horizontal en vertical input axis.
    /// </summary>
    private void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 movement = new Vector3(horizontal, 0.0f, vertical);

            if (horizontal != 0 && vertical != 0)
                rigidbodyComponent.AddForce(movement * movementSpeed * 0.7071f);
            else
                rigidbodyComponent.AddForce(movement * movementSpeed);

            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    private void CreateRaycastPlane()
    {
        if (raycastPlaneCollider != null)
            return;

        GameObject raycastPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        raycastPlaneCollider = raycastPlane.GetComponent<MeshCollider>();
        Destroy(raycastPlane.GetComponent<MeshRenderer>());

        raycastPlane.transform.localScale += new Vector3(10f, 0f, 10f);
        raycastPlane.gameObject.name = "PlayerRaycastPlane";
        raycastPlaneCollider.convex = true;
        raycastPlaneCollider.isTrigger = true;

        raycastPlane.transform.SetParent(transform);
    }
}
