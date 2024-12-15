using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour, IPlatform
{
    public float fallWait = 2f;
    public float destroyWait = 1f;

    private Rigidbody2D rigidBody;
    /* [SerializeField]
    private GameObject platform; */
    bool isFalling;

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private IEnumerator Fall()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallWait);
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, destroyWait);
    }

    public void PlatformAction()
    {
        Debug.Log("In falling platform action");
        StartCoroutine(Fall());
    }
}
