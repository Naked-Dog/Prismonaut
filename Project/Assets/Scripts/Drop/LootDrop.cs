using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Loot
{
    public GameObject collectablePrefab;
    public int amount;
}

public class LootDrop : MonoBehaviour
{
    [SerializeField] private Loot[] loot;

    public void DropLoot(Transform spawnPoint)
    {
        foreach (Loot rewards in loot)
        {
            for (int i = 0; i < rewards.amount; i++)
            {
                GameObject reward = Instantiate(rewards.collectablePrefab);
                reward.GetComponent<IDropable>()?.Drop(spawnPoint);
            }
        }
    }
}
