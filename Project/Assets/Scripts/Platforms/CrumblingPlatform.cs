using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour, IPlatform
{
    [SerializeField] private float crumbleWait = 1f;
    [SerializeField] private float respawnWait = 2f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isCrumbling = false;

    private BoxCollider2D boxCollider;
    private PlatformType _platformType = PlatformType.CrumblingPlatform;

    public PlatformType PlatformType { get => _platformType; set => _platformType = value; }

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private IEnumerator Crumble()
    {
        isCrumbling = true;
        yield return new WaitForSeconds(crumbleWait);
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(respawnWait);
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
        isCrumbling = false;
    }

    private IEnumerator DestroyPlatform()
    {
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(respawnWait);
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
    }

    public void PlatformAction(Player2DController player2DController)
    {
        if (isCrumbling) return;
        Debug.Log("In crumbling platform action");
        if (player2DController.form == FormState.Square && player2DController.isUsingPower)
        {
            StartCoroutine(DestroyPlatform());
        }
        else
        {
            StartCoroutine(Crumble());
        }
    }
}
