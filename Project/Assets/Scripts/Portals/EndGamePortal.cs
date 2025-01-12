using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGamePortal : MonoBehaviour, IPlayerPowerInteractable
{
    private AudioManager audioManager;
    [SerializeField] private Collider2D collider2D;

    public void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (playerState.activePower == PlayerSystem.Power.Circle)
        {
            Debug.Log("EndGame");
            collider2D.isTrigger = true;
        }
    }

    private void Awake()
    {
        //audioManager = new AudioManager(gameObject, GetComponent<PortalSoundList>(), GetComponent<AudioSource>());
    }

    private void Start()
    {
        //audioManager.PlayAudioClip("Idle", true);
    }
}
