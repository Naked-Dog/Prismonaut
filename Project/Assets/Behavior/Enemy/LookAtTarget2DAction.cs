using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Look At Target 2D", story: "[Self] looks at [Target]", category: "Action/Navigation", id: "346783f90377f4dd3e5d4c4a4221be16")]
public partial class LookAtTarget2DAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private float RunDirection; 

    protected override Status OnStart()
    {
        RunDirection = Mathf.Sign(Target.Value.transform.position.x - Self.Value.transform.position.x);

        Vector3 localScale = Self.Value.transform.localScale;
        localScale.x = -RunDirection * Mathf.Abs(localScale.x);
        Self.Value.transform.localScale = localScale; // ✅ Apply the change

        return Status.Success;
    }
}

