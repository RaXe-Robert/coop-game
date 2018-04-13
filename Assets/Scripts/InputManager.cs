using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputManager : MonoBehaviour {
    
    Dictionary<string, KeyCode> buttonKeys;
    
    private void OnEnable()
    {
        buttonKeys = new Dictionary<string, KeyCode>
        {
            //TODO: Reading these from a user preferences file
            ["Jump"] = KeyCode.Space,
            ["Left"] = KeyCode.LeftArrow,
            ["Right"] = KeyCode.RightArrow
        };
    }

    public bool GetButtonDown( string buttonName)
    {
        //TODO: Pause game when in text fields here.


        if(buttonKeys.ContainsKey(buttonName) == false)
        {
            Debug.LogError("InputManager::GetButtonDown -- No button named: " + buttonName);
            return false;
        }

        return Input.GetKeyDown(buttonKeys[buttonName]);
    }

    public string[] GetButtonNames()
    {
        return buttonKeys.Keys.ToArray();
    }

    public string GetKeyNameForButton(string buttonName)
    {
        if (buttonKeys.ContainsKey(buttonName) == false)
        {
            Debug.LogError("InputManager::GetKeyNameForButton -- No button named: " + buttonName);
            return "N/A";
        }

        return buttonKeys[buttonName].ToString();
    }

    public void SetButtonForKey( string buttonName, KeyCode keyCode)
    {
        buttonKeys[buttonName] = keyCode;
    }
}
