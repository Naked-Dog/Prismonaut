using UnityEngine;

[CreateAssetMenu(fileName = "Camera", menuName = "ScriptableObjects/Player/CameraValues", order = 1)]
public class CameraScriptable : ScriptableObject
{
    [Header("Timers")]
    public float FallingTime = 1.0f;
}
