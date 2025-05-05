using NUnit.Framework.Interfaces;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    [SerializeField]
    private float topPlatformHeight = 5;
    private readonly float lavaHeightDiff = 4;
    private readonly float timeToDie = 2;
    private bool isPlatformDead = false;
    public bool isFalling = false;
    private Transform lavaTransform;
    private Rigidbody2D rb;

    [SerializeField]
    private PlatformManager platformManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void Start()
    {
        platformManager = PlatformManager.Instance;
        lavaTransform = platformManager.lavaManager.transform;
    }

    void Update()
    {
        if(transform.position.y + topPlatformHeight < lavaTransform.position.y + lavaHeightDiff && !isPlatformDead)
        {
            PlatformManager.Instance.RemovePlatform(this);
            isPlatformDead = true;
            Destroy(gameObject, timeToDie);
        }
    }

    public void StartFalling(float grativyScale)
    {
        isFalling = true;
        rb.gravityScale = grativyScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            platformManager.PlayerSteppedOnPlatform(this);
        }
    }
}