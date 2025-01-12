using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : Enemy, IPlayerPowerInteractable
{
    [SerializeField] PlayerSystem.Power weakness;
    public Transform mouthTransform;
    public Transform projectileOrigin;

    public void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (playerState.activePower == weakness)
        {
            Damage(1);
        }
    }
}
