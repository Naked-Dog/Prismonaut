using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : Enemy
{
    [SerializeField] PlayerSystem.Power weakness;
    public override void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (playerState.activePower == weakness)
        {
            Damage(1);
        }
    }
}
