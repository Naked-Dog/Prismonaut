using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Agent] attacks [Target] after reaching [Collider]", category: "Action", id: "59d206075694382899a735dece7b5175")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Collider;

    private ColliderDetector attackCollider;  

    protected override Status OnStart()
    {
        attackCollider = Collider.Value.GetComponent<ColliderDetector>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var target = attackCollider.GetTarget("Player");
        if(target == null) return Status.Running;

        Target.Value = target.gameObject;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

