using System;
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
        ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.GetComponentInChildren<IInteractable>() != null)
            {
                var interactable = (MonoBehaviour)hit.transform.GetComponentInChildren<IInteractable>();
                if (Input.GetMouseButtonDown(0))
                {
                    if (Vector3.Distance(transform.position, interactable.transform.position) <= interactable.InteractionRange)
                    {
                        interactable.GetComponent<IInteractable>().Interact();
                    }
                }
            }
        }
    }
}
