using System;
using System.Collections.Generic;
using UnityEngine;

public class LavaManager : MonoBehaviour
{
    [SerializeField] private Transform lavaTransform;
    [SerializeField] private List<LavaPositions> lavaPositions = new List<LavaPositions>();
    [SerializeField] private ShakeScriptable shakeProfile;

    private float initialHeight;
    private float finalHeight;
    private float riseDuration;
    private float timeElapsed;
    private int lavaPosIndex;

    private bool eventStarted;
    public bool eventFinished { get; private set; }

    public static LavaManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeLava();
    }

    private void Start()
    {
        PlayLavaSound();
    }

    private void Update()
    {
        if (eventStarted)
            HandleLavaRise();
    }

    public void Reset()
    {
        lavaPosIndex = 0;
        timeElapsed = 0;
        eventStarted = false;
        eventFinished = false;

        if (lavaPositions.Count > 0)
        {
            initialHeight = lavaPositions[0].position;
            lavaTransform.position = new Vector3(lavaTransform.position.x, initialHeight, lavaTransform.position.z);
        }
        else
        {
            initialHeight = lavaTransform.position.y;
        }
    }

    public void StartLava()
    {
        if (eventStarted) return;
        eventStarted = true;
    }

    public void MoveToNextLavaGoal()
    {
        if (lavaPosIndex < lavaPositions.Count - 1)
        {
            lavaPosIndex++;
            PrepareNextRise();
        }
        else
        {
            StopEvent();
        }
    }

    public void Shake()
    {
        ShakeManager.Instance.CameraShake(shakeProfile);
        AudioManager.Instance.Play2DSound(LevelEventsSoundsEnum.Earthquake);
    }

    public void FinishEvent()
    {
        if (lavaPosIndex >= lavaPositions.Count - 1) return;

        eventFinished = true;
        eventStarted = true;
        lavaPosIndex = lavaPositions.Count - 1;
        PrepareNextRise();
    }


    private void InitializeLava()
    {
        initialHeight = lavaTransform.position.y;
        lavaTransform.position = new Vector3(lavaTransform.position.x, initialHeight, lavaTransform.position.z);
    }

    private void PlayLavaSound()
    {
        AudioManager.Instance.Play3DSoundAttached(LevelEventsSoundsEnum.Lava, lavaTransform, true);
    }

    private void HandleLavaRise()
    {
        if (lavaPosIndex >= lavaPositions.Count) return;

        riseDuration = lavaPositions[lavaPosIndex].time;
        finalHeight = lavaPositions[lavaPosIndex].position;
        timeElapsed += Time.deltaTime;

        if (timeElapsed <= riseDuration)
        {
            float riseAmount = Mathf.Lerp(initialHeight, finalHeight, timeElapsed / riseDuration);
            lavaTransform.position = new Vector3(lavaTransform.position.x, riseAmount, lavaTransform.position.z);
        }
        else
        {
            SetLavaAtGoal();
        }
    }

    private void PrepareNextRise()
    {
        timeElapsed = 0;
        initialHeight = lavaTransform.position.y;
    }

    private void SetLavaAtGoal()
    {
        lavaTransform.position = new Vector3(lavaTransform.position.x, finalHeight, lavaTransform.position.z);
        initialHeight = finalHeight;
        timeElapsed = 0;

        if (lavaPosIndex >= lavaPositions.Count - 1)
        {
            StopEvent();
        }
        else
        {
            lavaPosIndex++;
            riseDuration = lavaPositions[lavaPosIndex].time;
            finalHeight = lavaPositions[lavaPosIndex].position;
        }
    }

    private void StopEvent()
    {
        eventStarted = false;
        eventFinished = true;
        AudioManager.Instance?.Stop(LevelEventsSoundsEnum.Lava);
    }
}

[Serializable]
public class LavaPositions
{
    public float position;
    public float time;
}