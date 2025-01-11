using System;
using UnityEngine;

public enum ChestStates
{
    Close,
    Open
}

public class Chest : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [HideInInspector] public ChestStates state = ChestStates.Close;
    [SerializeField] private LootDrop lootDrop;
    [SerializeField] private SpriteRenderer chestSprite;

    public void ChestInteraction()
    {
        if (state == ChestStates.Open) return;

        OpenChest();
    }

    private void OpenChest()
    {
        state = ChestStates.Open;
        lootDrop.DropLoot(spawnPoint);
        chestSprite.color = Color.red;
        GetComponent<Collider2D>().enabled = false;
    }
}
