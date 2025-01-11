using UnityEngine;

[CreateAssetMenu(fileName = "Camera", menuName = "ScriptableObjects/Player/CameraValues", order = 1)]
public class CameraScriptable : ScriptableObject
{
    [Header("Positions")]
    public Vector2 RegularPosition;
    public Vector2 FallingPosition;
    public Vector2 LookingDownPosition;
    public Vector2 DialoguePosition;
    public float RegularDistance;
    public float FallingDistance;
    public float DialogueDistance;
}
