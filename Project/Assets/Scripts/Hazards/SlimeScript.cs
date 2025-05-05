using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeScript : MonoBehaviour
{
    [Header("Speed Parameters")]
    public float minTriggerSpeed = 2f;
    public float maxTriggerSpeed = 6f;

    [Header("Bounce Parameters")]
    public float maxBounceSpeed = 11f;
    public float chargeTime = 0.2f;

    private readonly HashSet<Rigidbody2D> busy = new HashSet<Rigidbody2D>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;
        if (rb == null || busy.Contains(rb)) return;

        // 1) Velocidad relativa justo antes del choque
        Vector2 relVel = collision.relativeVelocity;

        // 2) Normal del punto de contacto
        Vector2 normal = collision.contacts[0].normal;

        // 3) Determinar eje de rebote: si |normal.y| > |normal.x| → suelo/techo (eje Y),
        //    si no → paredes (eje X)
        bool bounceOnY = Mathf.Abs(normal.y) > Mathf.Abs(normal.x);

        // 4) Medir sólo la componente relevante de la velocidad
        float impactSpeed = bounceOnY
            ? Mathf.Abs(relVel.y)
            : Mathf.Abs(relVel.x);

        if (impactSpeed < minTriggerSpeed) return;

        // 5) Mapear [minTriggerSpeed, maxTriggerSpeed] → [0,1]
        float t = Mathf.InverseLerp(minTriggerSpeed, maxTriggerSpeed, impactSpeed);
        // 6) Calcular la velocidad de rebote en [0, maxBounceSpeed]
        float bounceImpulse = Mathf.Lerp(0f, maxBounceSpeed, t);

        // 7) Lanza la coroutine de carga y rebote
        StartCoroutine(ChargeAndBounce(rb, normal, relVel, bounceImpulse));
    }

    private IEnumerator ChargeAndBounce(Rigidbody2D rb, Vector2 normal, Vector2 relVel, float bounceImpulse)
    {
        busy.Add(rb);

        // ——— Fase “pegado” al slime ———
        rb.linearVelocity = Vector2.zero;
        RigidbodyType2D rbType = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Kinematic;

        yield return new WaitForSeconds(chargeTime);

        // ——— Fase de rebote ———
        rb.bodyType = rbType;

        Vector2 newVel;
        if (Mathf.Abs(normal.y) > Mathf.Abs(normal.x))
        {
            // Rebote en Y: invertimos sólo Y, X queda igual
            float dirY = Mathf.Sign(normal.y); // +1 si es suelo, -1 si es techo
            newVel = new Vector2(
                relVel.x,
                bounceImpulse * dirY
            );
        }
        else
        {
            // Rebote en X: invertimos sólo X, Y queda igual
            float dirX = Mathf.Sign(normal.x); // +1 si es pared izquierda, -1 pared derecha
            newVel = new Vector2(
                bounceImpulse * dirX,
                relVel.y
            );
        }

        rb.AddForce(newVel, ForceMode2D.Impulse);

        // Pequeña espera antes de permitir otro rebote
        yield return new WaitForSeconds(0.1f);
        busy.Remove(rb);
    }
}