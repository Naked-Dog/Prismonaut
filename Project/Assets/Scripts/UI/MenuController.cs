using System;
using System.Collections;
using DG.Tweening;
using PlayerSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject solidBG;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameMenuPanel;
    [SerializeField] private InputActionAsset playerActions;
    private InputActionMap playerUIMap => playerActions.FindActionMap("UI");

    public static MenuController Instance { get; private set; }

    public EventBus eventBus;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic(GetMusicClipByCurrentActiveScene());
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(FadeTransition(sceneName));
    }

    public void ResetScene()
    {
        StartCoroutine(FadeTransition(SceneManager.GetActiveScene().name));
    }

    public IEnumerator FadeTransition(string sceneName)
    {
        solidBG.SetActive(true);
        Image backgroundImage = solidBG.GetComponent<Image>();

        Tween fadeIn = backgroundImage.DOFade(1, 0.5f);
        yield return fadeIn.WaitForCompletion();

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        MusicEnum nextSceneMusic = GetMusicClipByScene(sceneName);
        bool IsNextSceneMusicActuallyPlaying = AudioManager.Instance.IsSameMusicPlaying(nextSceneMusic);

        if (nextSceneMusic == MusicEnum.None || !IsNextSceneMusicActuallyPlaying)
        {
            AudioManager.Instance.StopMusic();
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SetMenuDisplay(sceneName);
        // yield return new WaitForSeconds(1);

        if(!IsNextSceneMusicActuallyPlaying)
        {
            AudioManager.Instance.PlayMusic(GetMusicClipByCurrentActiveScene());
        }

        Tween fadeOut = backgroundImage.DOFade(0, 0.5f);
        yield return fadeOut.WaitForCompletion();
        solidBG.SetActive(false);
    }

    public IEnumerator FadeInSolidPanel()
    {
        solidBG.SetActive(true);
        Image backgroundImage = solidBG.GetComponent<Image>();
        Tween fadeIn = backgroundImage.DOFade(1, 0.5f);
        yield return fadeIn.WaitForCompletion();
    }

    public IEnumerator FadeOutSolidPanel()
    {
        Image backgroundImage = solidBG.GetComponent<Image>();
        Tween fadeOut = backgroundImage.DOFade(0, 0.5f).OnComplete(() => { solidBG.SetActive(false); });
        yield return fadeOut.WaitForCompletion();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void DisplayPanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void ShowPausePanel(RequestPause e)
    {
        gameMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HidePausePanel(RequestUnpause e)
    {
        Time.timeScale = 1f;
        gameMenuPanel.SetActive(false);
    }

    private void SetMenuDisplay(string sceneName)
    {
        switch (sceneName)
        {
            case "Menu":
                if (mainMenuPanel == null) mainMenuPanel = GameObject.Find("MainMenuPanel");
                mainMenuPanel.SetActive(true);
                gameMenuPanel.SetActive(false);
                playerUIMap.Enable();
                break;
            default:
                if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
                gameMenuPanel.SetActive(false);
                break;
        }
    }

    public void SetEvents(EventBus bus)
    {
        eventBus = bus;
        eventBus.Subscribe<RequestPause>(ShowPausePanel);
        eventBus.Subscribe<RequestUnpause>(HidePausePanel);
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        eventBus.Publish(new RequestRespawn());
    }

    public void ResumeGame()
    {
        eventBus.Publish(new RequestUnpause());
    }

    private MusicEnum GetMusicClipByCurrentActiveScene()
    {
        return GetMusicClipByScene(SceneManager.GetActiveScene().name);
    }

    private MusicEnum GetMusicClipByScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Menu":
                return MusicEnum.Menu;
            case "Beta_Level_1":
            case "Beta_Boss_1":
                return MusicEnum.Level1;
            case "FinalScene":
                return MusicEnum.FinalCinematic;
            default:
                return MusicEnum.None;
        }
    }

    public void GameReturnToMenu()
    {
        eventBus.Publish(new RequestUnpause());
        ChangeScene("Menu");
    }
}
