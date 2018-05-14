using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public enum Sound
    {
        PRESS
    }
    
    public AudioSource AudioSource;
    public AudioClip PressClip;

    private static UISoundManager _instance;
    public static UISoundManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    public void PlaySound(Sound sound)
    {
        switch (sound)
        {
            case Sound.PRESS:
                AudioSource.PlayOneShot(PressClip);
                break;
            
            default:
                throw new NotImplementedException();
        }
    }
}
