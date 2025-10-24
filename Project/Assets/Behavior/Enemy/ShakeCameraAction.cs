using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Cinemachine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShakeCamera", story: "Shakes camera with [Force] for [Time]", category: "Action", id: "cce6f932280eb454cda053c0787fb37b")]
public partial class ShakeCameraAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Force;
    [SerializeReference] public BlackboardVariable<float> Time;

    protected override Status OnStart()
    {
        ShakeManager.Instance.CustomShake(force: Force.Value, duration: Time.Value);
        return Status.Success;
    }
}

