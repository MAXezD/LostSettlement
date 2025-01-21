using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WendigoAnimEvents : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] AudioSource evoScream;
    [SerializeField] AudioSource walk1;
    [SerializeField] AudioSource walk2;
    [SerializeField] AudioSource attack;
    [SerializeField] AudioSource stunned;
    
    public enum SoundList
    {
        Walk1,
        Walk2,
        Evolve,
        Attack,
        Stunned,
    }

    public void PlaySound(SoundList sound)
    {
        switch (sound)
        {
            case SoundList.Walk1:
                walk1.Play();
                break;
            case SoundList.Walk2:
                walk2.Play(); 
                break;
            case SoundList.Attack: 
                attack.Play();
                break;
            case SoundList.Stunned:
                stunned.Play(); 
                break;
            case SoundList.Evolve:
                evoScream.Play(); 
                break;

        }
    }
}
