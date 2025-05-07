using UnityEngine;

[CreateAssetMenu(fileName = "SlimeSettings", menuName = "Slime/Settings", order = 1)]
public class SlimeSettings : ScriptableObject
{
    [Header("Speed Parameters")]
    public float minTriggerSpeed = 2f;
    public float maxTriggerSpeed = 6f;

    [Header("Bounce Parameters")]
    public float maxBounceSpeed = 11f;
    public float oppositeDirMult = 0.5f;

    [Header("Charge Parameters")]
    public float baseChargeTime = 0.2f;
    public float minChargeTime = 0;
}