using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Bull Attack", story: "[Agent] Attacks [Target]", category: "Action", id: "55b2083aa11255a8de1e7fdb08572979")]
public partial class BullAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> RunDirection = new BlackboardVariable<float>(0f);

    protected override Status OnStart()
    {
        RunDirection.Value = Mathf.Sign(Target.Value.transform.position.x - Agent.Value.transform.position.x);

        Vector3 localScale = Agent.Value.transform.localScale;
        localScale.x = -RunDirection.Value * Mathf.Abs(localScale.x);
        Agent.Value.transform.localScale = localScale;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;
        Vector3 position = Agent.Value.transform.position;
        position.x += RunDirection.Value * Speed.Value * Time.deltaTime;
        Agent.Value.transform.position = position;

        Vector3 scale = Agent.Value.transform.localScale;
        scale.x = -RunDirection.Value * Mathf.Abs(scale.x);
        Agent.Value.transform.localScale = scale;

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

