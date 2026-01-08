using UnityEngine;
using UnityEngine.InputSystem;

public class UIControllerGameplay : UIControllerBase
{
    private const string k_PLAYER_MAP = "Player";
    private const string k_UI_MAP = "UI";
    private const string k_PAUSE_ACTION = "Pause";

    [Header("Gameplay Panels")]
    [SerializeField] private PanelBase m_pausePanel;
    [SerializeField] private PanelBase m_settingsPanel;

    private InputActionMap m_gameplayMap;
    private InputAction m_pauseAction;

    protected override void Awake()
    {
        base.Awake();
        InitializePanels();
    }

    protected override void Start()
    {
        base.Start();
        SetupInput();
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        CleanupInput();

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void InitializePanels()
    {
        if (m_pausePanel != null)
            m_pausePanel.HideInstant();
        if (m_settingsPanel != null)
            m_settingsPanel.HideInstant();
    }

    private void SetupInput()
    {
        m_gameplayMap = InputActions.FindActionMap(k_PLAYER_MAP, true);
        m_pauseAction = m_gameplayMap.FindAction(k_PAUSE_ACTION, true);
        m_pauseAction.performed += OnPausePressed;
        m_pauseAction.Enable();
    }

    private void CleanupInput()
    {
        if (m_pauseAction != null)
        {
            m_pauseAction.performed -= OnPausePressed;
            m_pauseAction.Disable();
        }
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Playing:
                HandlePlayingState();
                break;
            case GameManager.GameState.Paused:
                HandlePausedState();
                break;
            case GameManager.GameState.Settings:
                HandleSettingsState();
                break;
        }
    }

    private void HandlePlayingState()
    {
        SwitchInputMap(k_UI_MAP, k_PLAYER_MAP);
        m_pausePanel?.Close();
        m_settingsPanel?.Close();
        ClearCurrentPanel();
    }

    private void HandlePausedState()
    {
        SwitchInputMap(k_PLAYER_MAP, k_UI_MAP);
        OpenPanel(m_pausePanel);
    }

    private void HandleSettingsState()
    {
        OpenPanel(m_settingsPanel);
    }

    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (m_pausePanel == null) return;
        GameManager.Instance.TogglePause();
    }

    public void ResumeGame() =>
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);

    public void OnSettingsButton() =>
        GameManager.Instance.SetGameState(GameManager.GameState.Settings);

    public void ReturnToPausePanel() =>
        GameManager.Instance.SetGameState(GameManager.GameState.Paused);

    public void ReturnToMenu()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        SceneLoader.Instance.LoadScene(SceneType.MainMenu);
    }

    protected override void OnCancel(InputAction.CallbackContext ctx)
    {
        var state = GameManager.Instance.CurrentGameState;

        if (state == GameManager.GameState.Settings)
            GameManager.Instance.SetGameState(GameManager.GameState.Paused);
        else if (state == GameManager.GameState.Paused)
            GameManager.Instance.SetGameState(GameManager.GameState.Playing);
    }

    private void SwitchInputMap(string mapToDisable, string mapToEnable)
    {
        InputActions.FindActionMap(mapToDisable, true)?.Disable();
        InputActions.FindActionMap(mapToEnable, true)?.Enable();
    }
}
