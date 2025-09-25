using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using PlayerSystem;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ColliderTagDetector", story: "[Collider] collides with object with tag: [Tag]", category: "Action", id: "0fdc4054fd671da2007dfa9e6006e9b7")]
public partial class ColliderTagDetectorAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Collider;
    [SerializeReference] public BlackboardVariable<string> Tag;
    [SerializeReference] public BlackboardVariable<bool> ReactToParry;

    private ColliderDetector detectionCollider;

    protected override Status OnStart()
    {
        if (Collider?.Value == null)
        {
            Debug.LogError("Collider is null or not assigned.");
            return Status.Failure;
        }

        detectionCollider = Collider.Value.GetComponent<ColliderDetector>();
        if (detectionCollider == null)
        {
            Debug.LogError("ColliderDetector component not found on the assigned Collider.");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (detectionCollider == null)
        {
            Debug.LogError("DetectionCollider is null. Ensure OnStart was successful.");
            return Status.Failure;
        }

        var target = detectionCollider.GetTarget(Tag?.Value);
        if (target == null) return Status.Running;

        if (target.CompareTag("Player") && ReactToParry?.Value == true)
        {
            var player = target.GetComponentInParent<PlayerBaseModule>();

            if (player != null && player.state.isParry)
            {
                return Status.Success;
            }

            return Status.Running;
        }

        return Status.Success;
    }
}
