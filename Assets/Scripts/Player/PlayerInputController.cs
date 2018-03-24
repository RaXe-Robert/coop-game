using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour {

	private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F))
            GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Crafting);

        if (Input.GetKeyDown(KeyCode.C))
            GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Equipment);

        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.B))
            GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Inventory);
    }
}
