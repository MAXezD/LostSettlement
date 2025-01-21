using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DeagleAnimationEvents : MonoBehaviour
{
    [SerializeField] AudioClip magOut;
    [SerializeField] AudioClip magIn;
    [SerializeField] AudioClip cocking;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void CallReload()
    {
        Gun.Instance.Reload();
    }
    public enum SoundList
    {
        MagOut,
        MagIn,
        Cocking
    }

    public void PlaySound(SoundList sound)
    {
        switch (sound)
        {
            case SoundList.MagIn:
                audioSource.PlayOneShot(magIn);
                break;
            case SoundList.MagOut:
                audioSource.PlayOneShot(magOut);
                break;
            case SoundList.Cocking:
                audioSource.PlayOneShot(cocking);
                break;

        }
    }

}
