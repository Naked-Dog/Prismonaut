using UnityEngine;

public enum KinematicDrillBehavior
{
    Drill,
    Bounce,
    Cancel
}

[CreateAssetMenu(fileName = "Drill Power", menuName = "ScriptableObjects/Player/Drill Power Constants", order = 1)]
public class PlayerDrillPowerScriptable : ScriptableObject
{
    [Header("Dodge Power")]
    public float dodgePowerForce = 12f;
    public float dodgePowerDuration = 0.7f;
    public float dodgePowerBreakForce = 2f;

    [Header("Shield Power")]
    public float shieldPowerDuration = 0.5f;

    [Header("Drill Power")]
    public float drillMinimalFirstVelocity = 4f;
    public float drillMinimalSecondVelocity = 8f;
    public float drillFirstPowerDuration = 0.5f;
    public float drillSecondPowerDuration = 2f;
    public float drillFirstSteeringAmount = 1f;
    public float drillSecondSteeringAmount = 4f;
    public KinematicDrillBehavior drillBehavior = KinematicDrillBehavior.Drill;
}