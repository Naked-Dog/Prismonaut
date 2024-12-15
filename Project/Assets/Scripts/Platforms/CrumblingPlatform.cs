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

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private IEnumerator Crumble()
    {
        isCrumbling = true;
        yield return new WaitForSeconds(crumbleWait);
        boxCollider.enabled = false;
        if (gameObject.transform.GetChild(gameObject.transform.childCount - 1).CompareTag("Player"))
        {
            gameObject.transform.GetChild(gameObject.transform.childCount - 1).transform.parent = null;
        }
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(respawnWait);
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
        isCrumbling = false;
    }

    public void PlatformAction()
    {
        if (isCrumbling) return;
        Debug.Log("In crumbling platform action");
        StartCoroutine(Crumble());
    }
}
