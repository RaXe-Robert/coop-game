using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputManager : MonoBehaviour {
    
    private Dictionary<string, KeyCode> changableButtonKeys;
    private Dictionary<string, KeyCode> staticButtonKeys;
        
    private void OnEnable()
    {
        changableButtonKeys = new Dictionary<string, KeyCode>
        {
            ["Up"] = KeyCode.W,
            ["Down"] = KeyCode.S,
            ["Left"] = KeyCode.A,
            ["Right"] = KeyCode.D,
            ["Crafting"] = KeyCode.C,
            ["Inventory"] = KeyCode.I,
            ["Equipment"] = KeyCode.F,   
            ["Left camera rotation"] = KeyCode.Q,
            ["Right camera rotation"] = KeyCode.E,
        };

        staticButtonKeys = new Dictionary<string, KeyCode>
        {
            ["Escape"] = KeyCode.Escape,
            ["Camera rotation"] = KeyCode.Mouse1,  
            ["Spawn item"] = KeyCode.R
        };
    }

    public float GetAxis(string axisName)
    {
        //Make sure buttons don't react when interface is open
        if (GameInterfaceManager.Instance.IsAnyInterfaceOpen())
        {
            return 0;
        }
        return Input.GetAxis(axisName);
    }

    public float GetAxisRaw(string axisName)
    {
        //Make sure buttons don't react when interface is open
        if (GameInterfaceManager.Instance.IsAnyInterfaceOpen())
        {
            return 0;
        }

        if (axisName.Equals("Horizontal"))
        {
            if (GetButton("Left") && !GetButton("Right"))
                return -1f;
            else if (GetButton("Right") && !GetButton("Left"))
                return 1f;
        }

        if (axisName.Equals("Vertical"))
        {
            if (GetButton("Up") && !GetButton("Down"))
                return 1f;
            else if (GetButton("Down") && !GetButton("Up"))
                return -1f;
        }

        return 0;
    }

    public bool GetButtonDown(string buttonName)
    {
        //Make sure buttons do react when interface is open

        if (changableButtonKeys.ContainsKey(buttonName) == true)
        {
            return Input.GetKeyDown(changableButtonKeys[buttonName]);
        }
        if (staticButtonKeys.ContainsKey(buttonName) == true)
        {
            return Input.GetKeyDown(staticButtonKeys[buttonName]);
        }
        return false;
    }

    public bool GetButton(string buttonName)
    {
        //Make sure buttons don't react when interface is open
        if (GameInterfaceManager.Instance.IsAnyInterfaceOpen())
        {
            return false;
        }

        if (changableButtonKeys.ContainsKey(buttonName) == true)
        {
            return Input.GetKey(changableButtonKeys[buttonName]);
        }
        if (staticButtonKeys.ContainsKey(buttonName) == true)
        {
            return Input.GetKey(staticButtonKeys[buttonName]);
        }
        return false;        
    }

    public string[] GetChangableButtonNames()
    {
        return changableButtonKeys.Keys.ToArray();
    }

    public string GetNameForChangableButton(string buttonName)
    {
        if (changableButtonKeys.ContainsKey(buttonName) == false)
        {
            return "N/A";
        }
        return changableButtonKeys[buttonName].ToString();
    }

    public void SetChangableButtonForKey( string buttonName, KeyCode keyCode)
    {
        changableButtonKeys[buttonName] = keyCode;
    }
}
