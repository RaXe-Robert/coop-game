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
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity) || EventSystem.current.IsPointerOverGameObject()) 
            
            return;
        var interactable = hit.transform.GetComponent<IInteractable>();
        if (interactable == null)
        {
            Tooltip.Instance.Hide();
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && interactable.IsInteractable)
            {
                PlayerNetwork.PlayerObject.GetComponent<PlayerMovementController>().StartInteraction(interactable);
            }

            if (interactable.TooltipText() != string.Empty)
            {
                //This show will not dissapear when hovering from world item to a UI element. {BUG}
                Tooltip.Instance.Show(interactable.TooltipText());
            }                
        }
    }
}
