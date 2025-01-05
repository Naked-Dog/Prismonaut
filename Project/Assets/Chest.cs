using System;
using UnityEngine;

public enum ChestStates
{
    Close,
    Open
}

[Serializable]
public class ChestLoot
{
    public GameObject collectablePrefab;
    public int amount;
}

public class Chest : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ChestLoot[] loot;
    [HideInInspector] public ChestStates state = ChestStates.Close;


    public void ChestInteraction()
    {
        if(state == ChestStates.Open) return;

        OpenChest();
    }

    private void OpenChest()
    {
        state = ChestStates.Open;

        foreach (ChestLoot rewards in loot)
        {
            for (int i = 0; i < rewards.amount; i++)
            {
                GameObject reward = Instantiate(rewards.collectablePrefab);
                reward.transform.position = spawnPoint.position;
            }
        }

        GetComponent<SpriteRenderer>().color = Color.red;
    }
}
