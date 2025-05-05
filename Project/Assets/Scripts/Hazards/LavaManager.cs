using System;
using System.Collections.Generic;
using UnityEngine;

public class LavaManager : MonoBehaviour
{
    [SerializeField]
    private float finalHeight = -1f;
    [SerializeField]
    private float initialHeight = -10f;
    [SerializeField]
    private float riseDuration = -1f;
    [SerializeField]
    private float timeElapsed = 0f;
    private bool eventStarted = false;
    private bool eventFinished = false;
    [SerializeField]
    private List<LavaPositions> lavaPositions = new List<LavaPositions>();

    private void Awake()
    {
        initialHeight = transform.position.y;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && !eventFinished)//temporal
        {
            if (eventStarted) moveToLavaGoal(); //temporal
            else StartLava();
        }

        if (eventStarted)
        {
            RiseLava();
        }
    }

    public void moveToLavaGoal()
    {
        lavaPositions.RemoveAt(0);
        timeElapsed = 0;
        initialHeight = transform.position.y;
    }

    public void StartLava()
    {
        if (eventStarted) return;
        eventStarted = true;
        if(PlatformManager.Instance) PlatformManager.Instance.StartPlatforms();
    }

    private void RiseLava()
    {
        timeElapsed += Time.deltaTime;

        riseDuration = lavaPositions[0].time;
        finalHeight = lavaPositions[0].position;

        if (timeElapsed <= riseDuration)
        {
            float riseAmount = Mathf.Lerp(initialHeight, finalHeight, timeElapsed / riseDuration);
            transform.position = new Vector3(transform.position.x, riseAmount, transform.position.z);
        }
        else
        {
            SetLavaGoal();
        }
    }
    private void SetLavaGoal()
    {
        transform.position = new Vector3(transform.position.x, finalHeight, transform.position.z);
        initialHeight = finalHeight;
        timeElapsed = 0;
        lavaPositions.RemoveAt(0);

        if (lavaPositions.Count <= 0)
        {
            eventStarted = false;
            eventFinished = true;
            return;
        }
    }
    public void FinishEvent()
    {
        if (lavaPositions.Count == 0) return;
        while (lavaPositions.Count > 1)
        {
            SetLavaGoal();
        }
    }
}

[Serializable]
public class LavaPositions
{
    public float position;
    public float time;
}