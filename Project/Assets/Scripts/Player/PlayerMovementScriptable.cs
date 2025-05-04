using UnityEngine;

[CreateAssetMenu(fileName = "Movement", menuName = "ScriptableObjects/Player/MovementValues", order = 1)]
public class PlayerMovementScriptable : ScriptableObject
{
    [Header("Movement")]
    public float horizontalGroundedForce = 150f;
    public float horizontalAirborneForce = 50f;
    public float groundBreak = 6f;
    public float airBreak = 2f;
    public float maxHorizontalVelocity = 8f;
    public float maximumYSpeed = 25;
    public float minimumYSpeed = -15;
    public float gravityScale = 3f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float jumpTime = 0.2f;
    public float gravity = 7f;
    public float groundCheckExtraHeight = 0.25f;
    public LayerMask groundLayerMask;

    [Header("Circle Power")]
    public float circlePowerForce = 15f;
    public float circlePowerDuration = 0.4f;
    public float circlePowerCooldown = 1f;

    [Header("Square Power")]
    public float squarePowerForce = -10f;
    public float squarePowerCooldown = 1f;
    public float spikeKnockbackForce = 1.5f;

    [Header("Triangle Power")]
    public float trianglePowerForce = 10f;
    public float trianglePowerDuration = 0.3f;
    public float trianglePowerCooldown = 1f;

    [Header("Power Collision Force")]
    public float oppositeForce = 5f;
}