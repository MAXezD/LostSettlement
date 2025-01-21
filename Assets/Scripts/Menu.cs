using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    
    [SerializeField] int index;
    [SerializeField] GameObject settings;
    bool isSettingOpen = false;
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(index);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void OpenSettings()
    {
        if (!isSettingOpen)
        {
            settings.SetActive(true);
            isSettingOpen = true;
        }
        else
        {
            settings.SetActive(false);
            isSettingOpen = false;
        }
    }
    public void SwitchQuality(int qualityIndex)
    {
        if (QualitySettings.GetQualityLevel() != qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex,true);
        }
    }
    public void CapFramerate()
    {
        Application.targetFrameRate = 60;
    }
    public void UnCapFramerate()
    {
        Application.targetFrameRate = -1;
    }
}

