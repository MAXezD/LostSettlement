using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] CanvasGroup[] textList;
    [SerializeField] int nextScene;
    [SerializeField] float waitDuration;
    [SerializeField] float fadeDuration;
    [SerializeField] GameObject LoaderUI;
    [SerializeField] Slider progressSlider;
    bool skippable;

    public void Start()
    {
        DisableText();
        StartCoroutine(doFade(waitDuration,fadeDuration));
    }
    void DisableText()
    {
        for (int i = 0; i < textList.Length; i++)
        {
            textList[i].alpha = 0;
        }
    }
    private IEnumerator doFade(float wait,float fade)
    {
        skippable = true;
        for (int i = 0; i < textList.Length; i++)
        {
            textList[i].DOFade(1f, fade);
            yield return new WaitForSeconds(wait);
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && skippable)
        {
            skippable = false;
            DisableText();
            StartCoroutine(LoadSceneCoroutine(nextScene));
        }
    }
    public IEnumerator LoadSceneCoroutine(int index)
    {
        progressSlider.value = 0;
        LoaderUI.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        asyncOperation.allowSceneActivation = false;
        float progress = 0;

        while (!asyncOperation.isDone)
        {
            progress = Mathf.MoveTowards(progress, asyncOperation.progress, Time.deltaTime);
            progressSlider.value = progress;
            if (progress >= 0.9f)
            {
                progressSlider.value = 1;
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
