using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour, IPlayerPowerInteractable
{
    public PlayerSystem.Power powerInteraction;
    public Transform switchTransform;
    public UnityEvent onSwitchActivation;
    [SerializeField] private SpriteRenderer buttonSR;
    [SerializeField] private Sprite buttonSprite;

    public void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (playerState.activePower == powerInteraction)
        {
            ActivateSwitch();
            onSwitchActivation.Invoke();
        }
    }

    private void ActivateSwitch()
    {
        switchTransform.position = new Vector3(switchTransform.position.x, switchTransform.position.y - 0.1f);
        tag = "Ground";
        buttonSR.sprite = buttonSprite;
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().color = Color.red;
    }
}
