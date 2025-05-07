using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeScript : MonoBehaviour
{
    [Header("Speed Parameters")]

    [SerializeField]
    private float minTriggerSpeed = 2f;
    [SerializeField]
    private float maxTriggerSpeed = 6f;

    [Header("Bounce Parameters")]

    [SerializeField]
    private float maxBounceSpeed = 11f;
    [SerializeField]
    private float oppositeDirMult = 0.5f;
    [SerializeField]
    private float chargeTime = 0.2f;

    private readonly HashSet<Rigidbody2D> busy = new HashSet<Rigidbody2D>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;
        if (rb == null || busy.Contains(rb)) return;

        Vector2 relativeSpeed = collision.relativeVelocity;
        Vector2 normal = collision.contacts[0].normal;
        bool bounceOnY = Mathf.Abs(normal.y) > Mathf.Abs(normal.x);

        float impactSpeed = bounceOnY ? Mathf.Abs(relativeSpeed.y) : Mathf.Abs(relativeSpeed.x);

        if (impactSpeed < minTriggerSpeed) return;

        float t = Mathf.InverseLerp(minTriggerSpeed, maxTriggerSpeed, impactSpeed);

        float bounceImpulse = Mathf.Lerp(0f, maxBounceSpeed, t);

        BounceValues2 bounceValues = new(rb, t, bounceOnY, normal, relativeSpeed, bounceImpulse);

        StartCoroutine(ChargeAndBounce(bounceValues));
    }

    private IEnumerator ChargeAndBounce(BounceValues2 bounceValues)
    {
        busy.Add(bounceValues.rb);

        bounceValues.rb.linearVelocity = Vector2.zero;
        RigidbodyType2D rbType = bounceValues.rb.bodyType;
        bounceValues.rb.bodyType = RigidbodyType2D.Kinematic;

        yield return new WaitForSeconds(chargeTime * bounceValues.timeMultiplier);

        bounceValues.rb.bodyType = rbType;

        Vector2 newVel;
        if (bounceValues.bounceOnY)
        {
            float dirY = -Mathf.Sign(bounceValues.normal.y);
            newVel = new Vector2(
                bounceValues.relativeSpeed.x * (1 + Mathf.Clamp(bounceValues.bounceImpulse, 0, oppositeDirMult)),
                bounceValues.bounceImpulse * dirY
            );
        }
        else
        {
            float dirX = -Mathf.Sign(bounceValues.normal.x);
            float oppDirMult = bounceValues.relativeSpeed.y > 0 ? oppositeDirMult : 0;
            newVel = new Vector2(
                bounceValues.bounceImpulse * dirX,
                bounceValues.relativeSpeed.y * (1 + Mathf.Clamp(bounceValues.bounceImpulse, 0, oppDirMult))
            );
        }

        bounceValues.rb.AddForce(newVel, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.1f);
        busy.Remove(bounceValues.rb);
    }
}

public class BounceValues2
{
    public Rigidbody2D rb;
    public float timeMultiplier;
    public bool bounceOnY;
    public Vector2 normal;
    public Vector2 relativeSpeed;
    public float bounceImpulse;

    public BounceValues2(Rigidbody2D rb, float timeMultiplier, bool bounceOnY, Vector2 normal, Vector2 relativeSpeed, float bounceImpulse)
    {
        this.rb = rb;
        this.timeMultiplier = timeMultiplier;
        this.bounceOnY = bounceOnY;
        this.normal = normal;
        this.relativeSpeed = relativeSpeed;
        this.bounceImpulse = bounceImpulse;
    }
}