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

    [Header("Drill Power")]
    public float drillMinimalFirstVelocity = 4f;
    public float drillMaxFirstVelocity = 4f;
    public float drillMinimalSecondVelocity = 8f;
    public float drillFirstPowerDuration = 0.5f;
    public float drillSecondPowerDuration = 2f;
    public float drillFirstSteeringAmount = 1f;
    public float drillSecondSteeringAmount = 4f;
    public float lightDrillDamagePerSecond = 2f;
    public float heavyDrillDamagePerSecond = 4f;
    public float heavyDrillSpeed = 2f;
}