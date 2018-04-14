using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputManager : MonoBehaviour {
    
    private Dictionary<string, KeyCode> buttonKeys;
    
    private void OnEnable()
    {
        buttonKeys = new Dictionary<string, KeyCode>
        {
            //TODO: The movement keys are still a Input.Getaxis in the PlayerInputController.cs
            //["Forward"] = KeyCode.W,
            //["Backward"] = KeyCode.S,
            //["Left"] = KeyCode.A,
            //["Right"] = KeyCode.D,
            ["Crafting"] = KeyCode.C,
            ["Inventory"] = KeyCode.I,

            //Maybe Don't make escape button changable?
            ["Escape"] = KeyCode.Escape,
            ["Equipment"] = KeyCode.F,
            ["Camera rotation"] = KeyCode.Mouse1,
            ["Left camera rotation"] = KeyCode.Q,
            ["Right camera rotation"] = KeyCode.E,
            ["Camera zoom"] = KeyCode.Mouse2
        };
    }

    public bool GetButtonDown( string buttonName)
    {
        //TODO: Pause game when in text fields here.
        if(buttonKeys.ContainsKey(buttonName) == false)
        {
            return false;
        }

        return Input.GetKeyDown(buttonKeys[buttonName]);
    }

    public bool GetButton(string buttonName)
    {
        //TODO: Pause game when in text fields here.
        if (buttonKeys.ContainsKey(buttonName) == false)
        {
            return false;
        }

        return Input.GetKey(buttonKeys[buttonName]);
    }    

    public string[] GetButtonNames()
    {
        return buttonKeys.Keys.ToArray();
    }

    public string GetKeyNameForButton(string buttonName)
    {
        if (buttonKeys.ContainsKey(buttonName) == false)
        {
            return "N/A";
        }

        return buttonKeys[buttonName].ToString();
    }

    public void SetButtonForKey( string buttonName, KeyCode keyCode)
    {
        buttonKeys[buttonName] = keyCode;
    }
}
