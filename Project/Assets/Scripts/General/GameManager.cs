using System;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    [SerializeField] private int collectedPrisms;
    [SerializeField] private int playerCharges;

    [Header("Diegetic Info")]
    [SerializeField] private List<DiegeticInfoType> diegeticInfoTypes = new List<DiegeticInfoType>();

    [Header("Scene Database")]
    [SerializeField] private SceneDatabase sceneDatabase;

    private const int INITIAL_PLAYER_CHARGES = 1;
    private const int INITIAL_COLLECTED_PRISMS = 0;

    public bool ShieldUnlocked { get; private set; }
    public bool DrillUnlocked { get; private set; }
    public bool DodgeUnlocked { get; private set; }
    public int CollectedPrisms => collectedPrisms;
    public int PlayerCharges => playerCharges;

    public event Action<int> OnPrismCollected;
    public event Action<Power> OnPowerUnlocked;
    public event Action<GameState> OnGameStateChanged;

    public enum GameState { Playing, Paused, Settings }
    private GameState currentGameState = GameState.Playing;
    public GameState CurrentGameState => currentGameState;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private bool IsGameplayScene(string sceneName)
    {
        var currentScene = SceneManager.GetActiveScene();
        foreach (var scene in sceneDatabase.Scenes)
        {
            if (scene.SceneName == currentScene.name)
            {
                return scene.Type.ToString().StartsWith("Level");
            }
        }
        return false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsGameplayScene(scene.name))
        {
            Destroy(gameObject);
            return;
        }

        InitializeLevel(scene.name);
    }

    private void InitializeLevel(string sceneName)
    {
        if (IsGameplayScene(sceneName))
        {
            AudioManager.Instance?.Play2DSound(EnvironmentSoundsEnum.Wind, true);

            var sceneEntry = sceneDatabase.Scenes.Find(s => s.SceneName == sceneName);
            if (sceneEntry?.Type == SceneType.Level1)
            {
                ResetPlayerProgress();
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (PrismsUIController.Instance != null)
            PrismsUIController.Instance.InitUI(collectedPrisms);

        if (PlayerBaseModule.Instance != null)
            PlayerBaseModule.Instance.SetCharges(playerCharges);
    }

    public void ReturnToMenu()
    {
        SetGameState(GameState.Playing);
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadScene(SceneType.MainMenu);
        Destroy(gameObject);
    }

    private void ResetPlayerProgress()
    {
        ShieldUnlocked = false;
        DrillUnlocked = false;
        DodgeUnlocked = false;
        collectedPrisms = INITIAL_COLLECTED_PRISMS;
        playerCharges = INITIAL_PLAYER_CHARGES;
    }

    public void GetPrism()
    {
        collectedPrisms++;
        playerCharges = PlayerBaseModule.Instance.state.maxCharges;

        PrismsUIController.Instance.UpdatePrismUI(collectedPrisms);
        OnPrismCollected?.Invoke(collectedPrisms);
    }

    public void UnlockPower(Power power)
    {
        switch (power)
        {
            case Power.Shield when !ShieldUnlocked:
                ShieldUnlocked = true;
                break;
            case Power.Drill when !DrillUnlocked:
                DrillUnlocked = true;
                break;
            case Power.Dodge when !DodgeUnlocked:
                DodgeUnlocked = true;
                break;
            default:
                return;
        }

        PlayerBaseModule.Instance?.powersModule.SetPowerAvailable(power);
        OnPowerUnlocked?.Invoke(power);
    }

    public void UnlockShieldPower() => UnlockPower(Power.Shield);
    public void UnlockDrillPower() => UnlockPower(Power.Drill);
    public void UnlockDodgePower() => UnlockPower(Power.Dodge);

    public void ShowDiegeticInfoByID(int id)
    {
        if (id < 0 || id >= diegeticInfoTypes.Count) return;
        DiegeticInfo.Instance.ShowDiegeticInfo(diegeticInfoTypes[id]);
    }

    public void SetGameState(GameState newState)
    {
        if (currentGameState == newState) return;

        currentGameState = newState;

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.Settings:
                Time.timeScale = 0f;
                break;
        }

        OnGameStateChanged?.Invoke(currentGameState);
    }

    public void TogglePause()
    {
        SetGameState(currentGameState == GameState.Playing ?
            GameState.Paused : GameState.Playing);
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetGameState(GameState.Playing);
    }
}


