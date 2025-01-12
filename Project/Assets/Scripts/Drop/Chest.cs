using System;
using UnityEngine;

public enum ChestStates
{
    Close,
    Open
}

public class Chest : MonoBehaviour, IPlayerPowerInteractable
{
    [SerializeField] private Transform spawnPoint;
    [HideInInspector] public ChestStates state = ChestStates.Close;
    [SerializeField] private LootDrop lootDrop;
    [SerializeField] private SpriteRenderer chestSprite;

    public void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (state == ChestStates.Open) return;

        if (playerState.activePower == PlayerSystem.Power.Circle)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        state = ChestStates.Open;
        lootDrop.DropLoot(spawnPoint);
        chestSprite.color = Color.red;
        GetComponent<Collider2D>().enabled = false;
    }
}
