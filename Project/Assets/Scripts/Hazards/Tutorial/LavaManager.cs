using System;
using System.Collections.Generic;
using UnityEngine;

public class LavaManager : MonoBehaviour
{
    [SerializeField] private Transform lavaTransform;
    [SerializeField] private Spikes lavaDamage;
    [SerializeField] private float finalHeight = -1f;
    [SerializeField] private float initialHeight = -10f;
    [SerializeField] private float riseDuration = -1f;
    [SerializeField] private float timeElapsed = 0f;
    private float startHeight;
    [SerializeField] private List<LavaPositions> lavaPositions = new List<LavaPositions>();
    [SerializeField] private ShakeScriptable shakeProfile;
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private int lavaPosIndex = 0;

    private bool eventStarted = false;
    private bool eventFinished = false;
    public static LavaManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        initialHeight = lavaTransform.position.y;
        startHeight = initialHeight;
    }

    private void Start()
    {
        AudioManager.Instance.Play3DSoundAttached(LevelEventsSoundsEnum.Lava, lavaTransform, true);
    }
    public void Reset()
    {
        lavaTransform.position = new Vector3(lavaTransform.position.x, startHeight);
        lavaPosIndex = 0;
        timeElapsed = 0;
        initialHeight = lavaTransform.position.y;
    }

    void Update()
    {
        if (eventStarted)
        {
            RiseLava();
        }
    }

    public void moveToLavaGoal()
    {
        lavaPosIndex++;
        timeElapsed = 0;
        initialHeight = lavaTransform.position.y;
        if (lavaPosIndex == lavaPositions.Count) eventStarted = false;
    }

    public void StartLava()
    {
        if (eventStarted) return;
        eventStarted = true;
        // if (PlatformManager.Instance) PlatformManager.Instance.StartSequence();
    }

    public void Shake()
    {
        ShakeManager.Instance.CameraShake(shakeProfile);
        AudioManager.Instance.Play2DSound(LevelEventsSoundsEnum.Earthquake);
    }

    private void RiseLava()
    {
        timeElapsed += Time.deltaTime;

        riseDuration = lavaPositions[lavaPosIndex].time;
        finalHeight = lavaPositions[lavaPosIndex].position;

        if (timeElapsed <= riseDuration)
        {
            float riseAmount = Mathf.Lerp(initialHeight, finalHeight, timeElapsed / riseDuration);
            lavaTransform.transform.position = new Vector3(lavaTransform.position.x, riseAmount);
        }
        else
        {
            SetLavaGoal();
        }
    }
    private void SetLavaGoal()
    {
        lavaTransform.position = new Vector3(lavaTransform.position.x, finalHeight);
        initialHeight = finalHeight;
        timeElapsed = 0;
        lavaPosIndex++;

        if (lavaPosIndex == lavaPositions.Count - 1)
        {
            eventStarted = false;
            eventFinished = true;
            AudioManager.Instance?.Stop(LevelEventsSoundsEnum.Lava);
            return;
        }
        if (lavaPosIndex == lavaPositions.Count) eventStarted = false;
    }
    public void FinishEvent()
    {
        if (lavaPosIndex == lavaPositions.Count - 1) return;

        lavaDamage.isLava = false;

        eventStarted = true;

        lavaPosIndex = lavaPositions.Count - 1;
        initialHeight = lavaTransform.position.y;
        timeElapsed = 0;
        riseDuration = lavaPositions[lavaPosIndex].time;
        finalHeight = lavaPositions[lavaPosIndex].position;
    }
}

[Serializable]
public class LavaPositions
{
    public float position;
    public float time;
}