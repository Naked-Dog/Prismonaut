using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UtilityAI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectTarget", story: "[Agent] detects [Target]", category: "Action", id: "818301574f088cdd8cf019602a3b0580")]
public partial class DetectTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private Sensor sensor;

    protected override Status OnStart()
    {
        sensor = Agent.Value.GetComponent<Sensor>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var target = sensor.GetClosestTarget("Player");
        if(target == null) return Status.Running;

        Target.Value = target.gameObject;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

