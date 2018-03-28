﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;

    private Ray ray;
    private RaycastHit hit;
    private bool isMine;
    private float interactionTimeout;

    private void Start()
    {
        playerCamera = GetComponent<PlayerCameraController>().CameraReference;
        isMine = GetComponent<PhotonView>().isMine;
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
            var interactable = hit.transform.GetComponentInChildren<IInteractable>();
            if (interactable == null)
            {
                Tooltip.Instance.Hide();
            }
            else if(interactable != null)
            {
                if (Input.GetMouseButtonDown(0) && interactable.IsInteractable())
                {
                    if (interactable.GetType() == typeof(WorldResource))
                        InteractWithWorldResource(interactable);
                    else if (interactable.GetType() == typeof(EnemyNPC))
                        AttackEnemy(interactable);
                    else
                        interactable.Interact(gameObject.transform.position);
                }

                if (interactable.TooltipText() != string.Empty)
                {
                    Tooltip.Instance.Show(interactable.TooltipText());
                }
            }
        }
    }

    private void InteractWithWorldResource(IInteractable interactable)
    {
        if (interactionTimeout <= 0)
        {
            WorldResource resource = interactable as WorldResource;
            EquipmentManager equipmentManager = GetComponent<EquipmentManager>();
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
        else
        {
            WorldNotificationsManager.Instance.ShowNotification(new WorldNotificationArgs(transform.position, "Not ready yet", 1), true);
        }
    }

    private void AttackEnemy(IInteractable interactable)
    {
        if(interactionTimeout <= 0)
        {
            Debug.Log("Attacking");
            PlayerStats stats = GetComponent<PlayerStats>();
            NPCBase enemy = interactable as NPCBase;
            enemy.TakeDamage(UnityEngine.Random.Range(stats.MinDamage, stats.MaxDamage));
            interactionTimeout = stats.TimeBetweenAttacks;
        }
    }
}
