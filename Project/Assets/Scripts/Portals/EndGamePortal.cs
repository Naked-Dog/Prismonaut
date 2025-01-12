using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGamePortal : MonoBehaviour, IPlayerPowerInteractable
{
    private AudioManager audioManager;
    [SerializeField] private Collider2D collider2D;
    [SerializeField] private GameObject player3DModel;


    public void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (playerState.activePower == PlayerSystem.Power.Circle)
        {
            Debug.Log("EndGame");
            EnterEndGamePortal(playerState.playerGameObject.transform.position);
            collider2D.isTrigger = true;
        }
    }

    private void EnterEndGamePortal(Vector3 playerPosition)
    {
        GameObject model3D = Instantiate(player3DModel, playerPosition, Quaternion.identity);
        model3D.transform.localScale = Vector3.one * 0.5f;
    }
}
