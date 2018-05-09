using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Wrapper for Unity's Input system so that we are able to remap buttons ingame.
/// </summary>
public static class InputManager {

    private static Dictionary<string, KeyCode> changableButtonKeys = new Dictionary<string, KeyCode>
    {
        ["Up"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("UpKey", KeyCode.W.ToString())),
        ["Down"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DownKey", KeyCode.S.ToString())),
        ["Left"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LeftKey", KeyCode.A.ToString())),
        ["Right"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RightKey", KeyCode.D.ToString())),
        ["Crafting"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("CraftingKey", KeyCode.C.ToString())),
        ["Inventory"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("InventoryKey", KeyCode.I.ToString())),
        ["Equipment"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("EquipmentKey", KeyCode.F.ToString())),
        ["Left camera rotation"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LeftCameraRotationKey", KeyCode.Q.ToString())),
        ["Right camera rotation"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RightCameraRotationKey", KeyCode.E.ToString())),
        ["Open Chat"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("ChatKey", KeyCode.Y.ToString()))
    };

    private static Dictionary<string, KeyCode> staticButtonKeys = new Dictionary<string, KeyCode>
    {
        ["Escape"] = KeyCode.Escape,
        ["Camera rotation"] = KeyCode.Mouse1,
        ["Spawn item"] = KeyCode.R,
        ["Send Chat"] = KeyCode.Return,
        ["Close Chat"] = KeyCode.Escape
    };

    public static float GetAxis(string axisName)
    {
        //Make sure buttons don't react when interface is open
        if (GameInterfaceManager.Instance.IsAnyInterfaceOpen() || CustomInRoomChat.Instance.input.isFocused == true)
            return 0;

        return Input.GetAxis(axisName);
    }

    public static float GetAxisRaw(string axisName)
    {
        //Make sure buttons don't react when interface is open
        if (GameInterfaceManager.Instance.IsAnyInterfaceOpen() || CustomInRoomChat.Instance.input.isFocused == true)
            return 0;

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

    public static bool GetButtonDown(string buttonName)
    {
        //Make sure buttons do react when interface is open
        if (CustomInRoomChat.Instance.input.isFocused == true)
        {
            return false;
        }

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

    public static bool GetButton(string buttonName)
    {
        //Make sure buttons don't react when interface is open
        if (GameInterfaceManager.Instance.IsAnyInterfaceOpen() || CustomInRoomChat.Instance.input.isFocused == true)
            return false;

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

    public static string[] GetChangableButtonNames()
    {
        return changableButtonKeys.Keys.ToArray();
    }

    public static string GetNameForChangableButton(string buttonName)
    {
        if (changableButtonKeys.ContainsKey(buttonName) == false)
            return "N/A";

        return changableButtonKeys[buttonName].ToString();
    }

    /// <summary>
    /// Bind a button to a keycode. Returns true if the button was succesfully bound.
    /// </summary>
    /// <param name="buttonName">The name of the button.</param>
    /// <param name="keyCode">The keycode that the button has to respond to.</param>
    /// <returns>If the button was succesfully changed.</returns>
    public static bool SetChangableButtonKey(string buttonName, KeyCode keyCode)
    {
        if (IsButtonAvailable(keyCode))
        {
            changableButtonKeys[buttonName] = keyCode;
            PlayerPrefs.SetString(buttonName.Replace(" ", "") + "Key", keyCode.ToString());
            return true;
        }
        return false;
    }

    private static bool IsButtonAvailable(KeyCode keyCode)
    {
        if (changableButtonKeys.ContainsValue(keyCode) || staticButtonKeys.ContainsValue(keyCode))
            return false;

        return true;
    }
}
