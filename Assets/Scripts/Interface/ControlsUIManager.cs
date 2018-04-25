using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ControlsUIManager : MonoBehaviour {
    
    public GameObject keyItemPrefab;
    public GameObject keyList;

    private string buttonToRebind = null;
    private Dictionary<string, Text> buttonToLabel;
    
    void Start () {
        string[] buttonNames = InputManager.Instance.GetChangableButtonNames();
        buttonToLabel = new Dictionary<string, Text>();

        for (int i = 0; i < buttonNames.Length; i++)
        {
            string bn = buttonNames[i];

            GameObject go = Instantiate(keyItemPrefab);
            go.transform.SetParent(keyList.transform);
            go.transform.localScale = Vector3.one;

            Text buttonNameText = go.transform.Find("ButtonName").GetComponent<Text>();
            buttonNameText.text = bn;            

            Text keyNameText = go.transform.Find("Button/KeyName").GetComponent<Text>();
            keyNameText.text = InputManager.Instance.GetNameForChangableButton(bn);
            buttonToLabel[bn] = keyNameText;

            Button keyBindButton = go.transform.Find("Button").GetComponent<Button>();
            keyBindButton.onClick.AddListener(() => { StartRebindFor(bn); });
        }
	}
	
	void Update () {
		if (buttonToRebind != null)
        {
            if (Input.anyKeyDown)
            {
                foreach(KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kc))
                    {
                        InputManager.Instance.SetChangableButtonForKey(buttonToRebind, kc);
                        break;
                    }
                }
            }
        }
	}

    void StartRebindFor(string buttonName)
    {
        buttonToRebind = buttonName;
        buttonToLabel[buttonToRebind].text = "Press the new button";
    }

    public void ButtonInUse(string buttonName)
    {
        buttonToRebind = buttonName;
        buttonToLabel[buttonToRebind].text = "Button already in use";
    }

    public void SetLabel(KeyCode kc)
    {
        buttonToLabel[buttonToRebind].text = kc.ToString();
        buttonToRebind = null;
    }
}
