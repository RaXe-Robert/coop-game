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

    private void Start()
    {
        playerCamera = GetComponent<PlayerCameraController>().CameraReference;
        isMine = GetComponent<PhotonView>().isMine;
    }

    private void Update()
    {
        if (!isMine)
            return;

        ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            var interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable == null && !EventSystem.current.IsPointerOverGameObject())
            {
                Tooltip.Instance.Hide();
            }
            else if(interactable != null)
            {
                if (Input.GetMouseButtonDown(0) && interactable.IsInteractable())
                {
                    interactable.Interact(gameObject.transform.position);
                }

                if (interactable.TooltipText() != string.Empty)
                {
                    Tooltip.Instance.Show(interactable.TooltipText());
                }
            }
        }
    }
}
