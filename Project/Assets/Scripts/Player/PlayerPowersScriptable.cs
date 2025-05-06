using UnityEngine;

[CreateAssetMenu(fileName = "Powers", menuName = "ScriptableObjects/Player/PowerConstants", order = 1)]
public class PlayerPowersScriptable : ScriptableObject
{
    [Header("Dodge Power")]
    public float dodgePowerForce = 12f;
    public float dodgePowerDuration = 0.7f;
    public float dodgePowerBreakForce = 2f;

    [Header("Shield Power")]
    public float shieldPowerDuration = 0.5f;
    public float parryDuration = 0.15f;
    public float reflectionDamage = 8f;

    [Header("Drill Power")]

    [Header("First stage")]
    public float drillMinimalFirstVelocity = 4f;
    public float drillMaxFirstVelocity = 8f;
    public float drillAceleration = 0.5f;

    [Header("Second stage")]
    public float drillMinimalSecondVelocity = 8f;
    public float drillFirstPowerDuration = 0.5f;
    public float drillSecondPowerDuration = 2f;
    public float drillFirstSteeringAmount = 1f;
    public float drillSecondSteeringAmount = 4f;

    [Header("Light")]
    public float lightDrillDamagePerSecond = 2f;
    public float lightObjectExitForce = 2f;
    public float lightPlayerExitForce = 2f;

    [Header("Heavy")]
    public float heavyDrillDamagePerSecond = 4f;
    public float heavyDrillSpeed = 2f;
    public float steerReturnTimer = 0.4f;
    public float heavyExitForceImpulse = 2f;

    [Header("General")]
    public float drillOppositeForce = 4f;
}