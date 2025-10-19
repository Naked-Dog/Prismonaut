using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum FormState
{
    Undefined = -1,
    Circle = 0,
    Square = 1,
    Triangle = 2
}

public enum LifeIconState
{
    Undefined,
    Normal,
    Damaged,
    Off,
}

public class GameManager : MonoBehaviour
{
    [Header("Collectables")]
    [SerializeField] private int collectedPrisms;
    [SerializeField] private int playerCharges;

    [SerializeField] private List<DiegeticInfoType> diegeticInfoTypes = new List<DiegeticInfoType>();

    public static GameManager Instance;

    public bool ShieldUnlocked { get; private set; }
    public bool DrillUnlocked { get; private set; }
    public bool DodgeUnlocked { get; private set; }

    const int initialPlayerCharges = 1;
    const int initialPlayerCollectedPrism = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Menu" ||
            SceneManager.GetActiveScene().name == "StartCinematic" ||
            SceneManager.GetActiveScene().name == "FinalScene")
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == "Beta_Level_1")
        {
            ShieldUnlocked = false;
            DrillUnlocked = false;
            DodgeUnlocked = false;
            collectedPrisms = initialPlayerCollectedPrism;
            playerCharges = initialPlayerCharges;
        }

        PrismsUIController.Instance?.InitUI(collectedPrisms);
        PlayerBaseModule.Instance?.SetCharges(playerCharges);
    }

    public void ShowDiegeticInfoByID(int id)
    {
        DiegeticInfo.Instance.ShowDiegeticInfo(diegeticInfoTypes[id]);
    }

    public void GetPrism()
    {
        collectedPrisms++;
        PrismsUIController.Instance.UpdatePrismUI(collectedPrisms);
        playerCharges = PlayerBaseModule.Instance.state.maxCharges;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This sucks, but just for hint trigger events. I hate this.
    public void UnlockShieldPower()
    {
        if (ShieldUnlocked) return;
        ShieldUnlocked = true;
        PlayerBaseModule.Instance?.powersModule.SetPowerAvailable(PlayerSystem.Power.Shield);
    }

    public void UnlockDrillPower()
    {
        if (DrillUnlocked) return;
        DrillUnlocked = true;
        PlayerBaseModule.Instance?.powersModule.SetPowerAvailable(PlayerSystem.Power.Drill);
    }

    public void UnlockDodgePower()
    {
        if (DodgeUnlocked) return;
        DodgeUnlocked = true;
        PlayerBaseModule.Instance?.powersModule.SetPowerAvailable(PlayerSystem.Power.Dodge);
    }
}


