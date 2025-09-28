using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance { get; private set; }
    public GameObject lavaManager;

    public List<LavaPlatform> lavaPlatforms = new();
    [SerializeField] private int platformIndex = -1;
    private float gravityScale = 0.15f;
    private bool playerOnPlatform = false;
    private bool eventStarted = false;
    private bool eventFinished = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        foreach (LavaPlatform lPlatform in lavaPlatforms)
        {
            lPlatform.InitPlatform(this, lavaPlatforms.Count);
        }
    }

    public void PlayerSteppedOnPlatform(LavaPlatform platform)
    {
        if (platformIndex == platform.GetPlatformIndex()) return;

        platformIndex = platform.GetPlatformIndex();
        foreach (LavaPlatform lPlatform in lavaPlatforms)
        {
            lPlatform.UpdateMovement(platformIndex);
        }
    }

    public void StartSequence()
    {
        foreach (LavaPlatform lPlatform in lavaPlatforms)
        {
            lPlatform.ResetTimeToMove(lavaPlatforms.Count);
            lPlatform.SetPlatformStart();
        }
    }

    public void FinishSequence()
    {
        foreach (LavaPlatform lPlatform in lavaPlatforms)
        {
            lPlatform.UpdateMovement(lavaPlatforms.Count + 3);
        }
    }
}