using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    private Camera playerCamera;

    private Ray ray;
    private RaycastHit hit;
    private bool isMine;
    private float interactionTimeout;

    private PlayerStatsComponent statsComponent;
    private EquipmentManager equipmentManager;
    private PlayerCombatController combatController;

    private void Start()
    {
        playerCamera = GetComponent<PlayerCameraController>().CameraReference;
        isMine = GetComponent<PhotonView>().isMine;
        statsComponent = GetComponent<PlayerStatsComponent>();
        equipmentManager = GetComponent<EquipmentManager>();
        combatController = GetComponent<PlayerCombatController>();
    }

    private void Update()
    {
        if (!isMine)
            return;

        if (interactionTimeout > 0)
            interactionTimeout -= Time.deltaTime;

        ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && !EventSystem.current.IsPointerOverGameObject())
        {
            //We hit ourself
            if (hit.transform == transform)
                return;

            //TODO: Handle tooltips

            //We aren't ready to interact yet.
            if (interactionTimeout > 0)
                return;

            HandleEnemies();
            HandleInteractables();
        }
    }

    private void HandleInteractables()
    {
        var interactable = hit.transform.GetComponent<IInteractable>();
        if (interactable != null && Input.GetMouseButtonDown(0) && interactable.IsInteractable())
        {
            //Check if we are interacting with a resource
            if (interactable.GetType() == typeof(WorldResource))
                InteractWithWorldResource(interactable);
            else
                interactable.Interact(transform.position);
        }
    }

    private void InteractWithWorldResource(IInteractable interactable)
    {
        WorldResource resource = interactable as WorldResource;
        if (equipmentManager.HasToolEquipped(resource.requiredToolToHarvest))
        {
            interactionTimeout = 2;
            interactable.Interact(transform.position);
        }
        else
        {
            WorldNotificationsManager.Instance.ShowNotification(new WorldNotificationArgs(transform.position, "Wrong tool", 1), true);
        }
    }

    private void HandleEnemies()
    {
        var enemy = hit.transform.GetComponent<IAttackable>();
        if (enemy != null && Input.GetMouseButtonDown(0) && Vector3.Distance(transform.position, enemy.Position) < 3)
        {
            enemy.TakeHit(combatController);
            interactionTimeout = statsComponent.TimeBetweenAttacks;
        }
    }
}
