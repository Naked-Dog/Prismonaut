using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour, IPlatform
{
    public float fallWait = 2f;
    public float destroyWait = 1f;
    public float respawnWait = 1f;
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D rigidBody;
    private bool isFalling = false;
    private Vector3 startingPosition;

    void Awake()
    {
        startingPosition = transform.position;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private IEnumerator Fall()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallWait);
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(destroyWait);
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(respawnWait);
        rigidBody.bodyType = RigidbodyType2D.Static;
        if (gameObject.transform.GetChild(gameObject.transform.childCount - 1).CompareTag("Player"))
        {
            gameObject.transform.GetChild(gameObject.transform.childCount - 1).transform.parent = null;
        }
        transform.position = startingPosition;
        spriteRenderer.enabled = true;
        isFalling = false;
    }

    public void PlatformAction()
    {
        if (isFalling) return;
        Debug.Log("In falling platform action");
        StartCoroutine(Fall());
    }
}
