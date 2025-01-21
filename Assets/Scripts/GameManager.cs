using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] int nextScene;

    [Header("UI")]
    public CanvasGroup blackScreen;
    public CanvasGroup objective;
    public Text displayNum;

    [Header("Pause")]
    public bool isPaused = false;
    [SerializeField] private GameObject pauseScreen;

    [Header("Objectives")]
    public int incinerated = 0;
    public int incineratedMax = 6;
    [SerializeField] GameObject keyItemPrefab;
    [SerializeField] GameObject SpawnPointsParent;
    [SerializeField] GameObject[] keyItems;
    [SerializeField] List<Transform> keySpots = new List<Transform>();
    public GameObject burnSpot;
    public GameObject fireEfx;
    Collider thisCollider;

    private void Awake()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        Wendigo.Instance.gameObject.SetActive(false);
        keyItems = new GameObject[incineratedMax];
        foreach (Transform child in SpawnPointsParent.transform)
        {
            keySpots.Add(child);
        }
        thisCollider = GetComponent<Collider>();
        Debug.Log($"{QualitySettings.GetQualityLevel()}");
    }
    private void Start()
    {
        blackScreen.DOFade(0f, 3f);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameObject.FindGameObjectWithTag("Player").activeInHierarchy)
        {
            Puase();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SpawnKeyItem();
            Wendigo.Instance.gameObject.SetActive(true);
            thisCollider.enabled = false;
        }
        
    }
    void SpawnKeyItem()
    {
        for (int j = 0; j < keyItems.Length; j++)
        {
            GameObject _item = Instantiate(keyItemPrefab);
            keyItems[j] = _item;
            //random the place that the key items spawn
            int _keySpot = Random.Range(0, keySpots.Count);
            _item.transform.position = keySpots[_keySpot].position;
            _item.transform.rotation = keySpots[_keySpot].rotation;
            keySpots.RemoveAt(_keySpot);
        }
    }
    public IEnumerator Incinerating(GameObject key)
    {
        if (incinerated == 0)
        {
            fireEfx.SetActive(true);
        }
        key.transform.position = burnSpot.transform.position;
        key.transform.rotation = burnSpot.transform.rotation;
        yield return new WaitForSeconds(4f);
        Destroy(key);
        incinerated++;

        Wendigo.Instance.Evolve(incinerated);

        displayNum.text = incinerated.ToString();
        objective.DOFade(1f, 3f);
        yield return new WaitForSeconds(7f);
        objective.DOFade(0f, 3f);

    }
    public void Win()
    {
        blackScreen.DOFade(1f, 3f);
        SceneManager.LoadScene(nextScene);
    }

    public void Puase()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }


}
