using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour {

    private PhotonView photonview;
    InputManager inputManager;

    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        photonview = GetComponent<PhotonView>();
    }

    private void Update ()
    {
        if (photonview.isMine)
        {
            if (inputManager.GetButtonDown( "Escape" ))
                GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.EscapeMenu);

            if (inputManager.GetButtonDown( "Crafting" ) && !GameInterfaceManager.Instance.IsInterfaceOpen(GameInterface.EscapeMenu))
                GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Crafting);

            if (inputManager.GetButtonDown( "Equipment" ) && !GameInterfaceManager.Instance.IsInterfaceOpen(GameInterface.EscapeMenu))
                GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Equipment);

            if (inputManager.GetButtonDown( "Inventory" ) && !GameInterfaceManager.Instance.IsInterfaceOpen(GameInterface.EscapeMenu))
                GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Inventory);
        }
    }
}
