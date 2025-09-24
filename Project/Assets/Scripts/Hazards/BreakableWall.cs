using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreakableWall : MonoBehaviour, IPlayerPowerInteractable
{
    public void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (playerState.activePower != PlayerSystem.Power.None)
        {
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        Destroy(gameObject);
    }
}
