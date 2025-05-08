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
    [SerializeReference] public BlackboardVariable<bool> IsCharging = new BlackboardVariable<bool>(false);
    [SerializeReference] public BlackboardVariable<float> ChargeTimer = new BlackboardVariable<float>(0f);
    [SerializeReference] public BlackboardVariable<float> ChargeDuration = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> RunDirection = new BlackboardVariable<float>(0f);

    protected override Status OnStart()
    {
        IsCharging.Value = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Target.Value == null)
        return Status.Failure;

    // Step 1: Lock in direction and start charge-up
    if (!IsCharging.Value)
    {
        RunDirection.Value = Mathf.Sign(Target.Value.transform.position.x - Agent.Value.transform.position.x);
        IsCharging.Value = true;
        ChargeTimer.Value = ChargeDuration.Value;

        // Flip sprite to face player
        Vector3 localScale = Agent.Value.transform.localScale;
        localScale.x = -RunDirection.Value * Mathf.Abs(localScale.x); // flip if needed
        Agent.Value.transform.localScale = localScale;

        // TODO: Play charge-up animation/sound here if needed
        return Status.Running;
    }

    // Step 2: Wait during charge-up
    if (ChargeTimer.Value > 0f)
    {
        ChargeTimer.Value -= Time.deltaTime;
        return Status.Running;
    }

    // Step 3: Charge in locked direction
    Vector3 position = Agent.Value.transform.position;
    position.x += RunDirection.Value * Speed.Value * Time.deltaTime;
    Agent.Value.transform.position = position;

    // Optional: Keep sprite flipped correctly during charge
    Vector3 scale = Agent.Value.transform.localScale;
    scale.x = -RunDirection.Value * Mathf.Abs(scale.x);
    Agent.Value.transform.localScale = scale;

    return Status.Running;
    }

    protected override void OnEnd()
    {
        IsCharging.Value = false;
    }
}

