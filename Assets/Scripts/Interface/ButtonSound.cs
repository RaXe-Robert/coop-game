using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{

	private Button button;
	
	// Use this for initialization
	void Start ()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(() => 
			UISoundManager.Instance.PlaySound(UISoundManager.Sound.PRESS));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
}
