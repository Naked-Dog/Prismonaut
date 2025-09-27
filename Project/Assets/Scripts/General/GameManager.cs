using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using PlayerSystem;
using UnityEngine;
using UnityEngine.InputSystem;
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
public enum ObstacleType
{
    Asteroid,
    Hollow,
    Block,
}

public enum PlatformType
{
    MovingPlatform,
    FallingPlatform,
    CrumblingPlatform,
}

public enum CollectableType
{
    Prism,
    Gem,
    Power,
}

public class GameManager : MonoBehaviour
{
    [Header("Collectables")]
    [SerializeField] private int levelTargetGems = 3;
    [SerializeField] private int collectedGems = 0;
    [SerializeField] private int collectedPrisms = 0;
    [SerializeField] private int playerCharges = 1;

    [Header("GameWin")]
    [SerializeField] private GameObject[] endGamePortals;
    [SerializeField] private List<DiegeticInfoType> diegeticInfoTypes = new List<DiegeticInfoType>();

    public static GameManager Instance;

    public bool ShieldUnlocked {get; private set; }
    public bool DrillUnlocked {get; private set; }
    public bool DodgeUnlocked {get; private set; }

    #region Old Code
    public static int maxLifes = 3;
    public static FormState startingForm = FormState.Square;
    #endregion

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
        if (SceneManager.GetActiveScene().name == "Menu" || SceneManager.GetActiveScene().name == "FinalScene") return;
        PrismsUIController.Instance?.InitUI(collectedPrisms);
        PlayerBaseModule.Instance?.SetCharges(playerCharges);
    }

    public void ShowDiegeticInfoByID(int id)
    {
        DiegeticInfo.Instance.ShowDiegeticInfo(diegeticInfoTypes[id]);
    }

    public void GetGem()
    {
        collectedGems++;
        Debug.Log("Current Gems: " + collectedGems);
        if (collectedGems == levelTargetGems)
        {
            Debug.Log("Game Cleared");
            StartCoroutine(EnableGameEndPortals());
        }
    }

    public void GetPrism()
    {
        collectedPrisms++;
        PrismsUIController.Instance.UpdatePrismUI(collectedPrisms);
        playerCharges = PlayerBaseModule.Instance.state.maxCharges;
    }

    private IEnumerator EnableGameEndPortals()
    {

        foreach (GameObject endGamePortal in endGamePortals)
        {
            endGamePortal.transform.localScale = Vector2.zero;
            endGamePortal.SetActive(true);
            InputActionMap playerInput = InputSystem.actions.FindActionMap("Player");
            playerInput.Disable();
        }

        yield return new WaitForSeconds(2f);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This sucks, but just for hint trigger events. I hate this.
    public void UnlockShieldPower()
    {
        ShieldUnlocked = true;
        PlayerBaseModule.Instance?.powersModule.SetPowerAvailable(PlayerSystem.Power.Shield);
    }

    public void UnlockDrillPower()
    {
        DrillUnlocked = true;
        PlayerBaseModule.Instance?.powersModule.SetPowerAvailable(PlayerSystem.Power.Drill);
    }

    public void UnlockDodgePower()
    {
        DodgeUnlocked = true;
        PlayerBaseModule.Instance?.powersModule.SetPowerAvailable(PlayerSystem.Power.Dodge);
    }
}


