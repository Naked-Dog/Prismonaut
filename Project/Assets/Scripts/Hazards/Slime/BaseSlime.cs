using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;

public abstract class BaseSlime : MonoBehaviour
{
    [SerializeField] protected SlimeSettings ss;

    protected readonly HashSet<Rigidbody2D> busy = new();
    private bool attackReceived;
    protected Coroutine currentRoutine;
    private BounceValues pendingBounce;
    protected RigidbodyType2D originalType;

    [SerializeField] protected Animator anim;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;
        if (rb == null || busy.Contains(rb)) return;

        // Calculate relative velocity and contact normal
        Vector2 relVel = collision.relativeVelocity;
        Vector2 normal = collision.contacts[0].normal;
        bool bounceOnY = Mathf.Abs(normal.y) > Mathf.Abs(normal.x);
        float impactSpeed = bounceOnY ? Mathf.Abs(relVel.y) : Mathf.Abs(relVel.x);
        if (impactSpeed < ss.minTriggerSpeed) return;

        // Interpolate bounce impulse based on impact speed
        float t = Mathf.InverseLerp(ss.minTriggerSpeed, ss.maxTriggerSpeed, impactSpeed);
        float bounceImpulse = Mathf.Lerp(0f, ss.maxBounceSpeed, t);

        pendingBounce = new BounceValues(rb, t, bounceOnY, normal, relVel, bounceImpulse, ss.oppositeDirMult);

        busy.Add(rb);
        attackReceived = false;
        currentRoutine = StartCoroutine(HandleBounceOrAttack(pendingBounce));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerBaseModule player = collision.attachedRigidbody.GetComponent<PlayerBaseModule>();
        if(player.state.isParry)
        {
            DoOnReflect(pendingBounce);
        }
    }

    private IEnumerator HandleBounceOrAttack(BounceValues bv)
    {
        Debug.Log(bv.bounceOnY);
        Debug.Log(bv.relativeSpeed);
        Debug.Log(bv.normal);
        Debug.Log(bv.bounceImpulse);
        Rigidbody2D rb = bv.rb;
        rb.linearVelocity = Vector2.zero;
        originalType = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Kinematic;

        float elapsed = 0f;
        float waitTime = Mathf.Max(ss.baseChargeTime * bv.timeMultiplier, ss.minChargeTime);
        PlayerBaseModule player = rb.GetComponent<PlayerBaseModule>();
        if (player && !bv.bounceOnY)
        {
            player.animationsModule.InvertPlayerFacingDirection();
        }

        anim.Play("BounceIn");

        while (elapsed < waitTime)
        {
            if (attackReceived)
            {
                rb.bodyType = originalType;
                DoOnReflect(bv);
                busy.Remove(rb);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.bodyType = originalType;

        anim.Play("BounceOut");

        DoBounce(bv);
        yield return new WaitForSeconds(0.1f);
        busy.Remove(rb);
    }

    public void ReceiveReflect()
    {
        attackReceived = true;
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (pendingBounce != null)
        {
            // Restore physics state and execute reflect logic
            Rigidbody2D rb = pendingBounce.rb;
            rb.bodyType = originalType;
            DoOnReflect(pendingBounce);
            busy.Remove(rb);
        }
    }

    protected virtual void DoBounce(BounceValues bv)
    {
        Vector2 newVel;
        if (bv.bounceOnY)
        {
            float dirY = -Mathf.Sign(bv.normal.y);
            newVel = new Vector2(
                bv.relativeSpeed.x * (1 + Mathf.Clamp(bv.bounceImpulse, 0, bv.oppositeDirMult)),
                bv.bounceImpulse * dirY
            );
        }
        else
        {
            float dirX = -Mathf.Sign(bv.normal.x);
            float oppMult = bv.relativeSpeed.y > 0 ? bv.oppositeDirMult : 0;
            newVel = new Vector2(
                bv.bounceImpulse * dirX,
                bv.relativeSpeed.y * (1 + Mathf.Clamp(bv.bounceImpulse, 0, oppMult))
            );
        }

        bv.rb.linearVelocity = Vector2.zero;
        bv.rb.AddForce(newVel, ForceMode2D.Impulse);
        pendingBounce = null;
    }

    protected abstract void DoOnReflect(BounceValues bv);

    [ContextMenu("Test ReceiveAttack")]
    private void TestAttack() => ReceiveReflect();


    // Internal class to hold bounce data
    protected class BounceValues
    {
        public Rigidbody2D rb;
        public float timeMultiplier;
        public bool bounceOnY;
        public Vector2 normal;
        public Vector2 relativeSpeed;
        public float bounceImpulse;
        public float oppositeDirMult;

        public BounceValues(
            Rigidbody2D rb,
            float timeMultiplier,
            bool bounceOnY,
            Vector2 normal,
            Vector2 relativeSpeed,
            float bounceImpulse,
            float oppositeDirMult
        )
        {
            this.rb = rb;
            this.timeMultiplier = timeMultiplier;
            this.bounceOnY = bounceOnY;
            this.normal = normal;
            this.relativeSpeed = relativeSpeed;
            this.bounceImpulse = bounceImpulse;
            this.oppositeDirMult = oppositeDirMult;
        }
    }
}