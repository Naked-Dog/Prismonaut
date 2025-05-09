using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance { get; private set; }
    public GameObject lavaManager;

    public List<PlatformScript> platforms = new List<PlatformScript>();
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

    void Update()
    {
        if(platforms.Count >= 1 && eventStarted && !eventFinished) MakePlatformsFall();//temporal
    }

    public void PlayerSteppedOnPlatform(PlatformScript platform)
    {
        if (!playerOnPlatform)
        {
            platformIndex = platforms.IndexOf(platform);
            playerOnPlatform = true;
        }
    }

    private void MakePlatformsFall()
    {
        if (platforms.Count > 0 && platforms[0] != platforms[platformIndex] && platformIndex != -1)
        {
            float gravityMultiplier = (platformIndex - platforms.IndexOf(platforms[0])) * 2;
            platforms[0].StartFalling(gravityScale * gravityMultiplier);
        }
    }

    public void StartPlatforms()//temporal
    {
        eventStarted = true;
    }
    public void FinishEvent()
    {
        if(platforms.Count != 0)
        {
            foreach (var platform in platforms)
            {
                platform.StartFalling(gravityScale * 4);
                Destroy(platform.gameObject, 4);
            }
        }
    }

    public void RemovePlatform(PlatformScript platform)
    {
        platforms.Remove(platform);
        if(platforms.Count == 0) eventFinished = true;
    }
}