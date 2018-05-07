using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerCameraController))]
[RequireComponent(typeof(StatsComponent))]
public class PlayerMovementController : Photon.MonoBehaviour
{
    private Rigidbody rigidbodyComponent;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;
    private bool interruptedPickup = false;

    [SerializeField] private LayerMask rotationLayerMask;
    [SerializeField] private LayerMask waterLayerMask;
    [SerializeField] private float mouseDeadZoneFromPlayer;

    private PlayerCameraController cameraController = null;
    private StatsComponent stats;

    public GameObject ItemToPickup { get; set; }

    private void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        rigidbodyComponent = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraController = GetComponent<PlayerCameraController>();
        stats = GetComponent<StatsComponent>();
    }

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
        
        if (ItemToPickup == null || interruptedPickup)
            StopAutoWalkToItem();
        else
            AutoWalkToItem();
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
        if (Physics.Raycast(ray, out hit, 1000f, rotationLayerMask.value | waterLayerMask.value))
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
        Vector3 cameraDirectionForward = cameraController.CameraReference.transform.forward;
        cameraDirectionForward.y = 0;
        Vector3 cameraDirectionRight = Quaternion.Euler(0, 90, 0) * cameraDirectionForward;

        Vector3 movementInput = (cameraDirectionForward * InputManager.GetAxisRaw("Vertical") + cameraDirectionRight * InputManager.GetAxisRaw("Horizontal"));

        if (movementInput != Vector3.zero)
        {
            interruptedPickup = true;
            rigidbodyComponent.AddForce(movementInput.normalized * stats.MovementSpeed);

            animator.SetBool("IsRunning", true);
        }
        else
            animator.SetBool("IsRunning", false);
    }

    /// <summary>
    /// When there is an item to pickup this method moves to the item and checks if it needs to interact with the item.
    /// As soon as the player is near the item it will pick up the item and reset the agent.
    /// </summary>
    private void AutoWalkToItem()
    {
        if (!agent.hasPath)
            agent.SetDestination(ItemToPickup.transform.position);

        if (Vector3.Distance(ItemToPickup.transform.position, transform.position) < ItemToPickup.GetComponent<ItemWorldObject>().pickupDistance)
        {
            ItemToPickup.GetComponent<ItemWorldObject>().Interact(transform.position);
            StopAutoWalkToItem();
        }
        else
            animator.SetBool("IsRunning", true);
    }

    /// <summary>
    /// Interrupts the agent and clears the ItemToPickup.
    /// </summary>
    private void StopAutoWalkToItem()
    {
        ItemToPickup = null;
        interruptedPickup = false;

        if (agent.hasPath)
            agent.ResetPath();
    }
}
