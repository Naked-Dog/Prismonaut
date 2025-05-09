using UnityEngine;
using Cinemachine;

[CreateAssetMenu(menuName ="CameraShake/New Profile")]
public class ShakeScriptable : ScriptableObject
{
    public float shakeTime = 0.2f;
    public Vector3 Velocity;
    public float shakeForce = 1;
    public CinemachineImpulseDefinition.ImpulseTypes impulseType;
    public CinemachineImpulseDefinition.ImpulseShapes impulseShape;
}
