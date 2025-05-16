using System.Collections.Generic;
using UnityEngine;

public class SpikeSpawnerManager : MonoBehaviour
{
    public static SpikeSpawnerManager Instance;
    [SerializeField] private List<SpikeSpawner> listActivators = new();
    [SerializeField] private List<SpikeSpawner> listFalling = new();
    private int stage = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        if(stage == 1) FirstStage();
        if(stage == 2) SecondStage();
    }

    private void FirstStage()
    {
        if (listActivators.Count == 0) return;
        foreach(SpikeSpawner spawner in listActivators)
        {
            spawner.HandlePrepareSpike();
        }
        listActivators.Clear();
    }
    private void SecondStage()
    {
        if (listFalling.Count == 0) return;
        foreach (SpikeSpawner spawner in listFalling)
        {
            spawner.HandlePrepareSpike();
        }
        listFalling.Clear();
    }
}
