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
        AudioManager.Instance.PlayMusic(GetMusicClip());
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
        AudioManager.Instance.StopMusic();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        setMenuDisplay(sceneName);
        // yield return new WaitForSeconds(1);

        AudioManager.Instance.PlayMusic(GetMusicClip());

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

    public void DisplayGamePanel(OnPauseInput e)
    {
        Debug.Log("Toggling Game Panel");
        DisplayPanel(gameMenuPanel);
    }

    private void setMenuDisplay(string sceneName)
    {
        switch (sceneName)
        {
            case "Menu":
                if (mainMenuPanel == null) mainMenuPanel = GameObject.Find("MainMenuPanel");
                mainMenuPanel.SetActive(true);
                gameMenuPanel.SetActive(false);
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
        eventBus.Subscribe<OnPauseInput>(DisplayGamePanel);
    }

    public void ResetGame()
    {
        eventBus.Publish(new RequestRespawn());
        eventBus.Publish(new OnPauseInput());
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game");
        eventBus.Publish(new RequestUnpause());
        eventBus.Publish(new OnPauseInput());
    }

    private MusicEnum GetMusicClip()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Menu":
                return MusicEnum.Menu;
            case "Beta_Level_1":
                return MusicEnum.Level1;
            case "Beta_Boss_1":
                return MusicEnum.None;
            case "FinalScene":
                return MusicEnum.FinalCinematic;
            default:
                return MusicEnum.None;
        }
    }
}
