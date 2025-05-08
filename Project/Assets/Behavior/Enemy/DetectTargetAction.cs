using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectTarget", story: "Collides with [Target] on [Collider]", category: "Action", id: "818301574f088cdd8cf019602a3b0580")]
public partial class DetectTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Collider;
    private ColliderDetector detectionCollider;

    protected override Status OnStart()
    {
        detectionCollider = Collider.Value.GetComponent<ColliderDetector>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var target = detectionCollider.GetTarget("Player");
        if(target == null) return Status.Running;

        Target.Value = target.gameObject;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

