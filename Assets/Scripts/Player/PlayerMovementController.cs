using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MovementSpeedComponent))]
public class PlayerMovementController : Photon.MonoBehaviour
{
    private Rigidbody rigidbodyComponent;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;
    public GameObject ItemToPickup { get; set; }
    public bool InterruptedPickup = false;

    [SerializeField] private LayerMask rotationLayerMask;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float mouseDeadZoneFromPlayer;

    private PlayerCameraController cameraController = null;
    private MovementSpeedComponent movementSpeedComponent = null;

    private void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        rigidbodyComponent = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraController = GetComponent<PlayerCameraController>();
        movementSpeedComponent = GetComponent<MovementSpeedComponent>();
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

        if(ItemToPickup != null)
        {
            if(InterruptedPickup)
            {
                ItemToPickup = null;
                agent.ResetPath();
                InterruptedPickup = false;
                return;
            }
            MoveToPosition();          
        }
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
            InterruptedPickup = true;
            rigidbodyComponent.AddForce(movementInput.normalized * movementSpeedComponent.Value);

            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }


    public void MoveToPosition()
    {
        agent.SetDestination(ItemToPickup.transform.position);
        animator.SetBool("IsRunning", true);

        if (Vector3.Distance(ItemToPickup.transform.position, transform.position) < ItemToPickup.GetComponent<ItemWorldObject>().pickupDistance)
        {
            ItemToPickup.GetComponent<ItemWorldObject>().Interact(transform.position);
            ItemToPickup = null;
            agent.ResetPath();            
        }
    }
}
