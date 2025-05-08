using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ColliderTagDetector", story: "[Collider] collides with object with tag: [Tag]", category: "Action", id: "0fdc4054fd671da2007dfa9e6006e9b7")]
public partial class ColliderTagDetectorAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Collider;
    [SerializeReference] public BlackboardVariable<string> Tag;
    private ColliderDetector detectionCollider;

    protected override Status OnStart()
    {
        detectionCollider = Collider.Value.GetComponent<ColliderDetector>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var target = detectionCollider.GetTarget(Tag.Value);

        if(target == null) return Status.Running;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

