using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStats))]
public class PlayerMovementController : Photon.MonoBehaviour
{
    private Rigidbody rigidbodyComponent;
    private Animator animator;

    [SerializeField] private LayerMask rotationLayerMask;

    [SerializeField] private float mouseDeadZoneFromPlayer;

    private PlayerCameraController cameraController = null;
    private PlayerStats stats;

    private void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraController = GetComponent<PlayerCameraController>();
        stats = GetComponent<PlayerStats>();
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

    /// <summary>
    /// Rotates the player object towards the mouse.
    /// </summary>
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
        Vector3 movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));

        if (movementInput != Vector3.zero)
        {
            rigidbodyComponent.AddForce(movementInput.normalized * stats.MovementSpeed * 0.7071f);

            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }
}
