using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ResetCollider", story: "Reset [Collider]", category: "Action", id: "22594cffadd3b0d66aaeeceaff945afd")]
public partial class ResetColliderAction : Action
{
    [SerializeReference] public BlackboardVariable<ColliderDetector> Collider;

    protected override Status OnStart()
    {
        if (Collider?.Value == null)
            return Status.Failure;

        Collider.Value.ResetCollider();
        return Status.Success;
    }
}

