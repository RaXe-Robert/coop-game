﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PhotonView))]
public class CustomInRoomChat : Photon.MonoBehaviour
{
    private enum State{
        Opening,
        Opened,
        TimerOn,
        Closing,
        Closed
    }

    public static CustomInRoomChat Instance { get; private set; }

    public Text content;
    public InputField input;
    public Button sendButton;
    public GameObject scrollView;

    private State state = State.Closed;
    private float waitTime = 10F;
    private float resetWaitTime;
    private float fadeScale = 0.1F;
    private CanvasGroup canvasGroup;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;  
    }

    private void Start()
    {
        if (PhotonNetwork.offlineMode == true)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
        
        resetWaitTime = waitTime;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Update()
    {        
        if (InputManager.GetButtonDown("Open Chat"))
            OpenChat();

        if(input.isFocused == true)
        {
            if(state != State.Opened)
            {
                state = State.Opening;
                StartCoroutine(FadeChat());
            }
        }

        if (EventSystem.current.currentSelectedGameObject == input.gameObject)
        {
            if (InputManager.GetButtonDown("Send Chat"))
            {
                SendMessage();
                SetCurrentInputToNull();
            }

            if (InputManager.GetButtonDown("Close Chat"))
                CloseChat();
        }
        else
        {
            if(state == State.Opened)
                ResetAndStartTimer();
        }
    }

    public void SendMessage()
    {
        if (input.text == null || input.text == "")
            return;
                
        photonView.RPC("Chat", PhotonTargets.All, input.text);
        ClearMessage();        
    }

    private void ClearMessage() => input.text = "";

    [PunRPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        string senderName = "anonymous";

        if (mi.sender != null)
        {
            senderName = string.IsNullOrEmpty(mi.sender.NickName) ?
                $"Player {mi.sender.ID}" : mi.sender.NickName;
        }

        AddLine(senderName + ": " + newLine);
    }

    private void CheckState()
    {
        switch (state)
        {
            case State.Closing:
                StopCoroutine(FadeChat());
                state = State.Opening;
                StartCoroutine(FadeChat());
                break;
            case State.Closed:
                state = State.Opening;
                StartCoroutine(FadeChat());
                break;
            default:
                ResetAndStartTimer();
                break;
        }
    }

    public void AddLine(string newLine)
    {
        content.text += newLine + "\n";

        // Make sure the scrollview gets scrolled down to the bottom of the chat.
        Canvas.ForceUpdateCanvases();
        scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        Canvas.ForceUpdateCanvases();

        CheckState();
    }

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        AddLine($"Player joined: {PlayerNickNameOrId(newPlayer)}");
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        AddLine($"Player left: {PlayerNickNameOrId(otherPlayer)}");
    }

    private string PlayerNickNameOrId(PhotonPlayer player)
    {
        return string.IsNullOrEmpty(player.NickName) ?
            $"Player {player.ID}" :
            player.NickName;
    }

    private void OpenChat()
    {
        input.enabled = true;
        EventSystem.current.SetSelectedGameObject(input.gameObject, null);
        state = State.Opening;
        StartCoroutine(FadeChat());
    }

    private void CloseChat()
    {
        ClearMessage();
        input.enabled = false;
        SetCurrentInputToNull();
        state = State.TimerOn;
        StartCoroutine(Timer());
    }

    private void SetCurrentInputToNull() => EventSystem.current.SetSelectedGameObject(null);

    /// <summary>
    /// This Fades the chat when the state is closing or opening.
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeChat()
    {
        while (state == State.Closing)
        {
            yield return new WaitForSeconds(0.1F);
            FadeOutChat();
        }        
        while (state == State.Opening)
        {
            yield return new WaitForSeconds(0.1F);
            FadeInChat();
        }
    }

    private void FadeInChat()
    {
        if (canvasGroup.alpha != 1)
            canvasGroup.alpha += fadeScale;
        else
        {
            state = State.Opened;
            StopCoroutine(FadeChat());
        }
    }

    private void FadeOutChat()
    {
        if (canvasGroup.alpha != 0)
            canvasGroup.alpha -= fadeScale;
        else
        {
            state = State.Closed;
            StopCoroutine(FadeChat());
        }        
    }

    private void ResetAndStartTimer()
    {
        StopCoroutine(Timer());
        ResetWaitTime();
        state = State.TimerOn;
        StartCoroutine(Timer());
    }

    private void ResetWaitTime() => waitTime = resetWaitTime;

    /// <summary>
    /// This starts a timer to let the chat disappear after a while. waitTime will reset when the chat is active or when a message appears.
    /// </summary>
    IEnumerator Timer()
    {        
        yield return new WaitForSeconds(1F);             
        while (state == State.TimerOn)
        {
            yield return new WaitForSeconds(1F);
            waitTime -= 1F;
            if(waitTime <= 0)
            {
                ResetWaitTime();
                state = State.Closing;            
                StartCoroutine(FadeChat());             
            }         
        }
    }
}