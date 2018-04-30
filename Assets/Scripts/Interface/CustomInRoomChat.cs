using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PhotonView))]
public class CustomInRoomChat : Photon.MonoBehaviour
{
    public static CustomInRoomChat Instance { get; private set; }

    public Text content;
    public InputField input;
    public Button sendButton;
    public GameObject scrollView;
    
    public bool IsVisible = true;
    public static readonly string ChatRPC = "Chat";

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void Update()
    {
        if (InputManager.GetButtonDown("Chat key 1") || InputManager.GetButtonDown("Chat key 2"))
        {            
            if (EventSystem.current.currentSelectedGameObject == input.gameObject)
            {
                SendMessage();
            }
            EventSystem.current.SetSelectedGameObject(input.gameObject, null);
            input.OnPointerClick(new PointerEventData(EventSystem.current));

            if (InputManager.GetButtonDown("Escape"))
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    public void SendMessage()
    {
        if (input.text == null)
            return;
        if (input.text == "")
            return;
                
        photonView.RPC("Chat", PhotonTargets.All, input.text);        
        input.text = "";
    }

    [PunRPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        string senderName = "anonymous";

        if (mi.sender != null)
        {
            senderName = string.IsNullOrEmpty(mi.sender.NickName) ?
                $"Player {mi.sender.ID}" : mi.sender.NickName;
        }

        AddLine(senderName + ": " + newLine + "\n");
    }

    public void AddLine(string newLine)
    {
        content.text += newLine;
        scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }
}