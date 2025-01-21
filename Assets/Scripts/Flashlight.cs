using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private KeyCode SwitchKey = KeyCode.T;
    private AudioSource audioSource;
    private GameObject _light;
    bool powerOn;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        _light = transform.GetChild(0).gameObject;
        _light.SetActive(false);
    }
    private void Update()
    {

        //turn on/off flashlight
        if (Input.GetKeyDown(SwitchKey)) SwitchingLight();

        if (Wendigo.Instance.startedAttack)
        {
            _light.SetActive(false);
            powerOn = false;
        }
    }
    void SwitchingLight()
    {
        audioSource.Play();
        if (!powerOn)
        {
            _light.SetActive(true);
            powerOn = true;
        }
        else
        {
            _light.SetActive(false);
            powerOn = false;    
        }
        Wendigo.Instance.ExtraSenses("Flashlight",powerOn);
    }

}
