using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputManager : MonoBehaviour {

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

    // Use this for initialization
    void Start () {
        
    }

    Dictionary<string, KeyCode> buttonKeys;
    
	// Update is called once per frame
	void Update () {
		
	}

    public bool GetButtonDown( string buttonName)
    {
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
}
