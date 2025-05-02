using PlayerSystem;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    [SerializeField]
    private LavaManager lavaManager;
    [SerializeField]
    private PlatformManager platformManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("AAAAA");
            lavaManager.FinishEvent();
            platformManager.FinishEvent();
        }
    }

}
