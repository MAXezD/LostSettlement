using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathControl : Singleton<DeathControl>
{
    [SerializeField] GameObject background;
    GameObject player;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        background.SetActive(false);
        
    }
    private void Update()
    {
        if (player.activeInHierarchy == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            background.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
