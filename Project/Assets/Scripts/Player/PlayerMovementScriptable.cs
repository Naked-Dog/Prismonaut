using UnityEngine;

[CreateAssetMenu(fileName = "Movement", menuName = "ScriptableObjects/Player/MovementValues", order = 1)]
public class PlayerMovementScriptable : ScriptableObject
{
    public float horizontalVelocity = 5;
    public float jumpVelocity = 5;
    public float jumpTime = 0.5f;
    public float gravity = 5;
    public float groundCheckExtraHeight = 0.25f;
    public LayerMask groundLayerMask;
}