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
        if (player.state.isParry)
        {
            DoOnReflect(pendingBounce);
        }
    }

    private IEnumerator HandleBounceOrAttack(BounceValues bv)
    {
        Rigidbody2D rb = bv.rb;

        if (rb == null || rb.Equals(null)) yield break;

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
        AudioManager.Instance.Play2DSound(SlimeSoundsEnum.Hit);
        anim.Play("BounceIn");
        //!DeactivatePowersBesideSquare

        while (elapsed < waitTime)
        {
            if (attackReceived && rb != null && !rb.Equals(null))
            {
                rb.bodyType = originalType;
                DoOnReflect(bv);
                busy.Remove(rb);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (rb != null && !rb.Equals(null))
        {
            rb.bodyType = originalType;
            AudioManager.Instance.Play2DSound(SlimeSoundsEnum.Bounce);
            anim.Play("BounceOut");

            DoBounce(bv);
            yield return new WaitForSeconds(0.1f);
            busy.Remove(rb);

            //THIS NEEDS TO BE IMPROVED, THIS IS SHIT
            // if (player)
            // {
            //     player.StopPlayerActions();

            //     yield return new WaitForSeconds(0.35f);

            //     player.ResumePlayerActions();
            // }
        }
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

    protected virtual void DoBounce(BounceValues bValues)
    {
        Vector2 newVel;
        if (bValues.bounceOnY)
        {
            float dirY = -Mathf.Sign(bValues.normal.y);
            newVel = new Vector2(
                bValues.relativeSpeed.x * (1 + Mathf.Clamp(bValues.bounceImpulse, 0, bValues.oppositeDirMult)),
                bValues.bounceImpulse * dirY
            );
        }
        else
        {
            float dirX = -Mathf.Sign(bValues.normal.x);
            float oppMult = bValues.relativeSpeed.y > 0 ? bValues.oppositeDirMult : 0;
            newVel = new Vector2(
                bValues.bounceImpulse * dirX,
                bValues.relativeSpeed.y * (1 + Mathf.Clamp(bValues.bounceImpulse, 0, oppMult))
            );
        }

        bValues.rb.linearVelocity = Vector2.zero;
        bValues.rb.AddForce(newVel, ForceMode2D.Impulse);
        pendingBounce = null;
    }

    private void OnDisable()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
        busy.Clear();
        pendingBounce = null;
    }

    protected virtual void DoOnReflect(BounceValues bv)
    {
        bv.bounceImpulse = ss.maxBounceSpeed;
    }

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