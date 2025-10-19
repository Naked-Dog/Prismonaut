using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShakeCamera", story: "Shakes Camera", category: "Action", id: "cce6f932280eb454cda053c0787fb37b")]
public partial class ShakeCameraAction : Action
{

    protected override Status OnStart()
    {
        Debug.Log("Shake Camera");
        ShakeManager.Instance.CameraShake();
        return Status.Success;
    }

}

