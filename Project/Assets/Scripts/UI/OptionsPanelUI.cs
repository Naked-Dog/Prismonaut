using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsPanelUI : PanelUI
{
    [SerializeField] private Selectable initialSelectableObject;
    [SerializeField] private List<OptionItemUI> optionItems;
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Button resumeButton;

    public AudioManager menuAudio;
    private GameDataManager gameDataManager;
    private MenuController menuController;
    private EventSystem eventSystem;

    private void Awake()
    {
        //menuAudio = new AudioManager(gameObject, GetComponent<MenuSoundList>(), GetComponent<AudioSource>());
    }

    private void Start()
    {
        gameDataManager = GameDataManager.Instance;
        menuController = MenuController.Instance;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        Init();
    }

    private void Init()
    {
        InitOptionItems();
        InitOptionsSelection();
        if(playButton) SetPlayButton();
        if(creditsButton) SetCreditsButton();
        if(exitButton) SetExitButton();
        if(resumeButton) SetResumeButton();
    }

    private void InitOptionItems()
    {
        if (optionItems.Count == 0) return;
        foreach (OptionItemUI option in optionItems)
        {
            option.ResetOptionItem();
        }
    }

    private void InitOptionsSelection()
    {
        if (!initialSelectableObject) return;
        if (!EventSystem.current) return;
        EventSystem.current.SetSelectedGameObject(initialSelectableObject.gameObject);
    }

    private void SetPlayButton()
    {
        playButton.onClick.AddListener(gameDataManager.NewGame);
        playButton.onClick.AddListener(StartCinematic);
    }

    private void StartCinematic()
    {
        menuController.ChangeScene("StartCinematic");
    }

    private void SetCreditsButton()
    {
        creditsButton.onClick.AddListener(ShowCredits);
        creditsButton.onClick.AddListener(SetSelectGameObjectInEventSystem);
    }

    private void ShowCredits()
    {
        menuController.DisplayPanel(creditsPanel);
    }

    private void SetSelectGameObjectInEventSystem()
    {
        eventSystem.SetSelectedGameObject(creditsPanel);
    }

    private void SetExitButton()
    {
        exitButton.onClick.AddListener(menuController.ExitGame);
    }

    private void SetResumeButton()
    {
        resumeButton.onClick.AddListener(menuController.ResumeGame);
    }
}
