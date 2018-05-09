using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private bool isMine;

    private Camera playerCamera;
    private PlayerCombatController combatController;
    private PlayerMovementController playerMovementController;

    private void Start()
    {
        playerCamera = GetComponent<PlayerCameraController>().CameraReference;
        isMine = GetComponent<PhotonView>().isMine;
        combatController = GetComponent<PlayerCombatController>();
        playerMovementController = PlayerNetwork.PlayerObject.GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        if (!isMine)
            return;

        ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity) || EventSystem.current.IsPointerOverGameObject())
            return;

        var tooltip = hit.transform.GetComponent<ITooltip>();
        if (tooltip != null && hit.transform != transform)
            Tooltip.Instance.Show(tooltip.TooltipText);
        else
            Tooltip.Instance.Hide();

        HandleInteractables();
        HandleEnemies();
    }

    private void HandleInteractables()
    {
        var interactable = hit.transform.GetComponent<IInteractable>();
        if (interactable == null)
            return;

        if (Input.GetMouseButtonDown(0) && interactable.IsInteractable)
            playerMovementController.StartInteraction(interactable);
    }

    private void HandleEnemies()
    {
        var enemy = hit.transform.GetComponent<IAttackable>();
        if (enemy == null && hit.transform != transform)
            return;

        if (Input.GetMouseButtonDown(0) && playerMovementController.CanInteract && Vector3.Distance(transform.position, enemy.Position) < 3)
        {
            enemy.TakeHit(combatController);
            playerMovementController.AddInteractionTimeout(combatController.TimeBetweenAttacks);
        }
    }
}
