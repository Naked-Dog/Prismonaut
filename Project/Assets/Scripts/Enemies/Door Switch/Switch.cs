using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
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
        this.tag = "Ground";
        buttonSR.sprite = buttonSprite;
        this.GetComponent<Collider2D>().enabled = false;
        this.GetComponentInChildren<SpriteRenderer>().color = Color.red;
    }
}
