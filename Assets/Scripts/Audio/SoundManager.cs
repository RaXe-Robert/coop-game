using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public enum Sound
    {
        PICKUP,
        EQUIP
    }

    public enum AttackSound
    {
        WOOD,
        STONE
    }

    public AudioSource AudioSource;

    [Header("Hold your mouse on the list for details")]
    [Tooltip("Place in the following order\nPICKUP sound\n EQUIP sound")]
    public AudioClip[] SoundClips = new AudioClip[2];
    [Tooltip("Place in the following order\nAttack WOOD sound\n Attack STONE sound")]
    public AudioClip[] AttackSoundClips = new AudioClip[2];

    private static SoundManager _instance;
    public static SoundManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    public void PlayAttackSound(AttackSound attackSound)
    {
        switch (attackSound)
        {
            case AttackSound.WOOD:
                AudioSource.PlayOneShot(AttackSoundClips[(int)AttackSound.WOOD]);
                break;
            case AttackSound.STONE:
                AudioSource.PlayOneShot(AttackSoundClips[(int)AttackSound.STONE]);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    public void PlaySound(Sound soundType)
    {
        switch (soundType)
        {
            case Sound.PICKUP:
                AudioSource.PlayOneShot(SoundClips[(int)Sound.PICKUP]);
                break;
            case Sound.EQUIP:
                AudioSource.PlayOneShot(SoundClips[(int)Sound.EQUIP]);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
