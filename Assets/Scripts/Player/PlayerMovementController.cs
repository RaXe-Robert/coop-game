﻿using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerStatsComponent), typeof(PlayerCameraController))]
public class PlayerMovementController : Photon.MonoBehaviour
{
    private Rigidbody rigidbodyComponent;
    private Animator animator;
    private NavMeshAgent agent;
    private bool interruptedPickup = false;
    private float interactionTimeout = 0f;
    private float pathUpdateTimeout = 0f;
    private bool isMoving = false;
    public bool IsFrozen = false;

    [SerializeField] private LayerMask rotationLayerMask;
    [SerializeField] private LayerMask waterLayerMask;

    private PlayerCameraController cameraController = null;
    private PlayerStatsComponent stats;
    private PlayerCombatController combatController;
    private Inventory inventory;

    public GameObject CurrentInteraction { get; set; }

    public void StartInteraction(GameObject interactable) =>
        CurrentInteraction = interactable;

    /// <summary>
    /// Returns true if the player has no more interaction timeout
    /// </summary>
    public bool CanInteract => interactionTimeout <= 0;

    /// <summary>
    /// Adds delay to the timeout which the player needs to wait before it can interact with certain things
    /// </summary>
    /// <param name="timeout"></param>
    public void AddInteractionTimeout(float timeout) =>
        interactionTimeout += timeout;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigidbodyComponent = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraController = GetComponent<PlayerCameraController>();
        stats = GetComponent<PlayerStatsComponent>();
        combatController = GetComponent<PlayerCombatController>();
        inventory = GetComponent<Inventory>();
    }

    private void Start()
    {
        if (photonView.isMine)
        {
            if (cameraController != null)
                cameraController.StartFollowing();
            else
                Debug.LogError("Missing CameraController Component on playerPrefab.");
        }
    }

    private void Update()
    {
        if (!photonView.isMine)
            return;

        if (IsFrozen)
        {
            if (!isMoving && CurrentInteraction != null)
                Interact();
            else if (isMoving)
                StopInteraction();
            return;
        }

        if (interactionTimeout > 0)
            interactionTimeout -= Time.deltaTime;

        if (pathUpdateTimeout > 0)
            pathUpdateTimeout -= Time.deltaTime;

        if (!isMoving)
            RotatePlayer();
        
        if (CurrentInteraction == null || interruptedPickup)
            StopInteraction();
        else
            HandleInteraction();
    }

    private void FixedUpdate()
    {
        if (!photonView.isMine)
            return;

        if (IsFrozen)
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
            transform.LookAt(lookPos);
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
            transform.LookAt(transform.position + movementInput.normalized);

            animator.SetBool("IsRunning", true);
            isMoving = true;
        }
        else
        {
            if (CurrentInteraction == null)
            {
                animator.SetBool("IsRunning", false);
                isMoving = false;
            }
        }
    }

    /// <summary>
    /// When there is an item to pickup this method moves to the item and checks if it needs to interact with the item.
    /// As soon as the player is near the item it will pick up the item and reset the agent.
    /// </summary>
    private void HandleInteraction()
    {
        var interactable = CurrentInteraction.GetComponent<IInteractable>();
        var enemy = CurrentInteraction.GetComponent<IAttackable>();

        if (interactable != null && interactable.InRange(transform.position))
            Interact();
        else if (enemy != null && Vector3.Distance(transform.position, enemy.GameObject.transform.position) < 3)
            Interact();
        else
        {
            if ((!agent.hasPath || pathUpdateTimeout <= 0) && agent.isOnNavMesh)
            {
                agent.SetDestination(CurrentInteraction.transform.position);
                pathUpdateTimeout = 0.1f;
                isMoving = true;
                animator.SetBool("IsRunning", true);
            }
        }
    }

    public void Interact()
    {
        var interactable = CurrentInteraction.GetComponent<IInteractable>();
        var enemy = CurrentInteraction.GetComponent<IAttackable>();
        var combatController = GetComponent<PlayerCombatController>();
        
        if (interactable != null && interactable.InRange(transform.position) )
        {
            if(CurrentInteraction.GetComponent<WorldResource>() != null)
                combatController.TriggerHitAnimation();
            
            interactable.Interact(gameObject, inventory.inventoryItems[inventory.hotBarSelection]);
            StopInteraction();
        }
        else if (enemy != null && Vector3.Distance(transform.position, enemy.GameObject.transform.position) < 3 && CanInteract)
        {
            combatController.TriggerHitAnimation();
            enemy.TakeHit(combatController);
            AddInteractionTimeout(combatController.TimeBetweenAttacks);
            StopInteraction();
        }
        else
            animator.SetBool("IsRunning", true);
    }

    /// <summary>
    /// Interrupts the agent and clears the ItemToPickup.
    /// </summary>
    private void StopInteraction()
    {
        CurrentInteraction = null;
        interruptedPickup = false;

        if (agent.hasPath)
        {
            isMoving = false;
            agent.ResetPath();
        }
    }
}
