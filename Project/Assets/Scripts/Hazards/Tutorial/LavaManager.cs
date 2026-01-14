using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaManager : MonoBehaviour
{
    [SerializeField] private Transform lavaTransform;
    [SerializeField] private List<LavaPositions> lavaPositions = new();
    [SerializeField] private ShakeScriptable shakeProfile;

    public static LavaManager Instance { get; private set; }

    private Coroutine currentRiseCoroutine;
    private int currentIndex;
    private bool isActive;

    private Vector3 cachedPosition;

    public bool IsActive => isActive;
    public bool IsFinished => currentIndex >= lavaPositions.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayLavaSound();
        Reset();
    }

    public void Reset()
    {
        StopCurrentCoroutine();

        currentIndex = 0;
        isActive = false;

        if (lavaPositions.Count > 0)
        {
            SetHeight(lavaPositions[0].position);
        }
    }

    public void StartLava()
    {
        if (isActive || IsFinished || lavaPositions.Count == 0) return;

        isActive = true;
        currentRiseCoroutine = StartCoroutine(RiseLavaSequence());
    }

    public void ForceFinish(float forceTime = 1f)
    {
        if (!isActive || lavaPositions.Count == 0) return;

        StopCurrentCoroutine();
        currentRiseCoroutine = StartCoroutine(ForceRiseToEnd(forceTime));
    }

    private void StopCurrentCoroutine()
    {
        if (currentRiseCoroutine != null)
        {
            StopCoroutine(currentRiseCoroutine);
            currentRiseCoroutine = null;
        }
    }

    private IEnumerator ForceRiseToEnd(float duration)
    {
        isActive = true;

        float startHeight = lavaTransform.position.y;
        float targetHeight = lavaPositions[^1].position;

        yield return MoveLava(startHeight, targetHeight, duration);

        currentIndex = lavaPositions.Count;
        isActive = false;

        AudioManager.Instance?.Stop(LevelEventsSoundsEnum.Lava);
    }

    private IEnumerator RiseLavaSequence()
    {
        while (currentIndex < lavaPositions.Count)
        {
            LavaPositions currentPos = lavaPositions[currentIndex];
            float startHeight = lavaTransform.position.y;

            yield return MoveLava(startHeight, currentPos.position, currentPos.time);

            currentIndex++;
        }

        isActive = false;
        AudioManager.Instance?.Stop(LevelEventsSoundsEnum.Lava);
    }

    private IEnumerator MoveLava(float startHeight, float targetHeight, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newHeight = Mathf.Lerp(startHeight, targetHeight, t);
            SetHeight(newHeight);
            yield return null;
        }

        SetHeight(targetHeight);
    }

    private void SetHeight(float y)
    {
        cachedPosition = lavaTransform.position;
        cachedPosition.y = y;
        lavaTransform.position = cachedPosition;
    }

    public void Shake()
    {
        ShakeManager.Instance?.CameraShake(shakeProfile);
        AudioManager.Instance?.Play2DSound(LevelEventsSoundsEnum.Earthquake);
    }

    private void PlayLavaSound()
    {
        AudioManager.Instance?.Play3DSoundAttached(LevelEventsSoundsEnum.Lava, lavaTransform, true);
    }
}

[Serializable]
public class LavaPositions
{
    public float position;
    public float time;
}
