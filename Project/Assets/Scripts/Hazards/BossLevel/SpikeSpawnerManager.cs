using System.Collections.Generic;
using UnityEngine;

public class SpikeSpawnerManager : MonoBehaviour
{
    public static SpikeSpawnerManager Instance;
    [SerializeField] private List<SpikeSpawner> listActivators = new();
    [SerializeField] private List<SpikeSpawner> listFalling = new();
    private int stage = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetNextStage()
    {
        stage++;
        if (stage == 1) FirstStage();
        if (stage == 2) SecondStage();
        if (stage == 3) ClearStage();
    }

    private void FirstStage()
    {
        if (listActivators.Count == 0) return;
        foreach (SpikeSpawner spawner in listActivators)
        {
        }
    }
    private void SecondStage()
    {
        if (listFalling.Count == 0) return;
        foreach (SpikeSpawner spawner in listFalling)
        {
        }
    }

    private void ClearStage()
    {
        foreach (var spawner in listActivators)
        {
        }

        foreach (var spawner in listFalling)
        {
        }
        stage = 0;
    }
}
