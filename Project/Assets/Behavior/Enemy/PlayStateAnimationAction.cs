using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayStateAnimation", story: "[AnimationController] plays [AnimationState] animation", category: "Action", id: "7b63b50f93d635da751cf5caa7ad517f")]
public partial class PlayStateAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<ScarabullAnimationController> AnimationController;
    [SerializeReference] public BlackboardVariable<ScarabullAnimationState> AnimationState;

    protected override Status OnStart()
    {
        if (AnimationController.Value == null)
        {
            Debug.LogError("AnimationController is not set.");
            return Status.Failure;
        }

        AnimationController.Value.SetState(AnimationState.Value);
        return Status.Success;
    }
}

