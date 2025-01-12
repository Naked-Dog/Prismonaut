using System.Collections;
using PlayerSystem;
using UnityEngine;

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


    [Header("Collectables")]
    [SerializeField] private int levelTargetGems = 3;
    [SerializeField] private int collectedGems = 0;
    [SerializeField] private int collectedPrisms = 0;

    [Header("GameWin")]
    [SerializeField] private GameObject[] endGamePortals;


    #region Old Code
    public static int maxLifes = 3;
    public static FormState startingForm = FormState.Square;
    #endregion

    private void Awake()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBaseModule>();
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
        yield return new WaitForSeconds(2f);
        foreach (GameObject endGamePortal in endGamePortals)
        {
            endGamePortal.SetActive(true);
        }
    }

}


