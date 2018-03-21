﻿using UnityEngine;
using System.Collections;

public class PlayerMovementController : Photon.MonoBehaviour
{
    private Rigidbody rigidbodyComponent;
    private Animator animator;

    [SerializeField] private LayerMask rotationLayerMask;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float mouseDeadZoneFromPlayer;

    private PlayerCameraController cameraController = null;

    private void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraController = GetComponent<PlayerCameraController>();
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
        if (Physics.Raycast(ray, out hit, 1000f, rotationLayerMask.value))
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
}
