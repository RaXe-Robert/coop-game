using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour {

    private PhotonView photonview;

    private void Start()
    {
        photonview = GetComponent<PhotonView>();
    }

    private void Update ()
    {
        if (photonview.isMine)
        {
            if (InputManager.Instance.GetButtonDown( "Escape" ))
                GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.EscapeMenu);

            if (InputManager.Instance.GetButtonDown( "Crafting" ) && !GameInterfaceManager.Instance.IsInterfaceOpen(GameInterface.EscapeMenu))
                GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Crafting);

            if (InputManager.Instance.GetButtonDown( "Equipment" ) && !GameInterfaceManager.Instance.IsInterfaceOpen(GameInterface.EscapeMenu))
                GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Equipment);

            if (InputManager.Instance.GetButtonDown( "Inventory" ) && !GameInterfaceManager.Instance.IsInterfaceOpen(GameInterface.EscapeMenu))
                GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Inventory);
        }
    }
}
