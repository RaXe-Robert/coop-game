﻿using System;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;

    private Ray ray;
    private RaycastHit hit;

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            var interactable = hit.transform.GetComponentInChildren<IInteractable>();
            if (interactable != null && interactable.IsInteractable())
            {
                interactable.Interact(gameObject.transform.position);
            }
        }
    }
}
