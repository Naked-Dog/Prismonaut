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
        if (m_pausePanel == null)
        {
            Debug.LogWarning($"[UIControllerGameplay] Pause panel not assigned on {gameObject.name}.");
        }
        if (m_settingsPanel == null)
        {
            Debug.LogWarning($"[UIControllerGameplay] Settings panel not assigned on {gameObject.name}.");
        }
        InitializePanels();
    }

    protected override void Start()
    {
        base.Start();
        SetupInput();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
        else
        {
            Debug.LogError("[UIControllerGameplay] GameManager.Instance is null.");
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        CleanupInput();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void InitializePanels()
    {
        m_pausePanel?.HideInstant();
        m_settingsPanel?.HideInstant();
    }

    private void SetupInput()
    {
        if (InputActions == null)
        {
            Debug.LogError("[UIControllerGameplay] InputActions not assigned.");
            return;
        }
        m_gameplayMap = InputActions.FindActionMap(k_PLAYER_MAP, true);
        if (m_gameplayMap == null)
        {
            Debug.LogError("[UIControllerGameplay] 'Player' ActionMap not found in InputActions.");
            return;
        }
        m_pauseAction = m_gameplayMap.FindAction(k_PAUSE_ACTION, true);
        if (m_pauseAction == null)
        {
            Debug.LogError("[UIControllerGameplay] 'Pause' action not found in 'Player' ActionMap.");
            return;
        }
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
            default:
                Debug.LogWarning($"[UIControllerGameplay] Unhandled GameState: {state}");
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
        if (m_pausePanel != null)
        {
            OpenPanel(m_pausePanel);
        }
        else
        {
            Debug.LogWarning("[UIControllerGameplay] Pause panel is null when trying to open.");
        }
    }

    private void HandleSettingsState()
    {
        if (m_settingsPanel != null)
        {
            OpenPanel(m_settingsPanel);
        }
        else
        {
            Debug.LogWarning("[UIControllerGameplay] Settings panel is null when trying to open.");
        }
    }

    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (m_pausePanel == null)
        {
            Debug.LogWarning("[UIControllerGameplay] Pause panel is null on pause input.");
            return;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePause();
        }
        else
        {
            Debug.LogError("[UIControllerGameplay] GameManager.Instance is null on pause input.");
        }
    }

    public void ResumeGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        }
        else
        {
            Debug.LogError("[UIControllerGameplay] GameManager.Instance is null on resume.");
        }
    }

    public void OnSettingsButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Settings);
        }
        else
        {
            Debug.LogError("[UIControllerGameplay] GameManager.Instance is null on settings.");
        }
    }

    public void ReturnToPausePanel()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Paused);
        }
        else
        {
            Debug.LogError("[UIControllerGameplay] GameManager.Instance is null on return to pause.");
        }
    }

    public void ReturnToMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Playing);
            SceneLoader.Instance.LoadScene(SceneType.MainMenu);
        }
        else
        {
            Debug.LogError("[UIControllerGameplay] GameManager.Instance is null on return to menu.");
        }
    }

    protected override void OnCancel(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[UIControllerGameplay] GameManager.Instance is null on cancel.");
            return;
        }
        var state = GameManager.Instance.CurrentGameState;
        if (state == GameManager.GameState.Settings)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Paused);
        }
        else if (state == GameManager.GameState.Paused)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        }
    }

    private void SwitchInputMap(string mapToDisable, string mapToEnable)
    {
        if (InputActions == null)
        {
            Debug.LogError("[UIControllerGameplay] InputActions is null in SwitchInputMap.");
            return;
        }
        InputActions.FindActionMap(mapToDisable, true)?.Disable();
        InputActions.FindActionMap(mapToEnable, true)?.Enable();
    }
}
