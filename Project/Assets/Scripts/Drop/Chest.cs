using System;
using System.Collections;
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
    [SerializeField] private Animator animator;

    public void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (state == ChestStates.Open) return;
        if (playerState.activePower == PlayerSystem.Power.Circle || playerState.activePower == PlayerSystem.Power.Square)
        {
            OpenChest();
            StartCoroutine(DropLoot());
            animator.SetBool("isOpen", true);
        }
    }

    private void OpenChest()
    {
        state = ChestStates.Open;
        GetComponent<Collider2D>().enabled = false;
    }

    private IEnumerator DropLoot()
    {
        yield return new WaitForSeconds(0.28f);
        lootDrop.DropLoot(spawnPoint);
    }
}
