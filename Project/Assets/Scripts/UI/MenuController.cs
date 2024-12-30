using System.Collections;
using DG.Tweening;
using PlayerSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject solidBG;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameMenuPanel;
    [SerializeField] private GameObject losePanel;
    public static MenuController Instance {get; private set;}

    private EventBus eventBus;
    private PlayerInput input;
    

    private void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void ChangeScene(string sceneName){
        StartCoroutine(FadeTransition(sceneName));
    }

    public void ResetScene(){
        StartCoroutine(FadeTransition(SceneManager.GetActiveScene().name));
    }
    
    public IEnumerator FadeTransition(string sceneName){
        solidBG.SetActive(true);
        Image backgroundImage = solidBG.GetComponent<Image>();

        Tween fadeIn = backgroundImage.DOFade(1,0.5f);
        yield return fadeIn.WaitForCompletion();

        mainMenuPanel.SetActive(false);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        setMenuDisplay(sceneName);
        yield return new WaitForSeconds(1);    

        Tween fadeOut = backgroundImage.DOFade(0,0.5f);
        yield return fadeOut.WaitForCompletion();
        solidBG.SetActive(false);
    }

    public void ExitGame(){
        Application.Quit();
    }

    public void DisplayPanel(GameObject panel){
        panel.SetActive(!panel.activeSelf);
    }

    public void DisplayLosePanel(){
        DisplayPanel(losePanel);  
    }

    public void DisplayGamePanel(PauseInputEvent e){
        DisplayPanel(gameMenuPanel);     
    }

    private void setMenuDisplay(string sceneName){
        switch(sceneName){
            case "Menu":
                mainMenuPanel.SetActive(true);
                gameMenuPanel.SetActive(false);
                losePanel.SetActive(false);
            break;
            default:
                mainMenuPanel.SetActive(false);
                gameMenuPanel.SetActive(false);
                losePanel.SetActive(false);
            break;
        }
    }

    public void setEvents(EventBus bus){
        eventBus = bus;
        eventBus.Subscribe<PauseInputEvent>(DisplayGamePanel);
    }
}
