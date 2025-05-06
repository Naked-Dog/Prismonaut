using System.Collections;
using DG.Tweening;
using PlayerSystem;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [Header("Player")]
    [SerializeField] private PlayerBaseModule player;
    [SerializeField] private int maxCharges = 2;


    [Header("Collectables")]
    [SerializeField] private int levelTargetGems = 3;
    [SerializeField] private int collectedGems = 0;
    [SerializeField] private int collectedPrisms = 0;

    [Header("GameWin")]
    [SerializeField] private GameObject[] endGamePortals;

    [Header("HUD")]
    [SerializeField] private HealthUIController healthUIController;
    [SerializeField] private PrismsUIController prismsUIController;
    [SerializeField] private ChargesUIController chargesUIController;


    #region Old Code
    public static int maxLifes = 3;
    public static FormState startingForm = FormState.Square;
    #endregion

    private void Awake()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBaseModule>();
        healthUIController.InitUI(maxLifes);
        prismsUIController.InitUI(collectedPrisms);
        chargesUIController.InitChargesUI(maxCharges);
    }

    public void GetGem()
    {
        collectedGems++;
        Debug.Log("Current Gems: " + collectedGems);
        if (collectedGems == levelTargetGems)
        {
            //clear level
            Debug.Log("Game Cleared");
            StartCoroutine(EnableGameEndPortals());
        }
    }

    public void GetPrism()
    {
        collectedPrisms++;
        Debug.Log("Current Prisms: " + collectedPrisms);
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

}


