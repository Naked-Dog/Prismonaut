using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Bull Attack",
    story: "[Agent] Charges forward in chosen direction",
    category: "Action",
    id: "55b2083aa11255a8de1e7fdb08572979")]
public partial class BullAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>();

    private float runDirection;
    private Rigidbody2D rb;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null)
            return Status.Failure;

        rb = Agent.Value.GetComponent<Rigidbody2D>();
        if (rb == null) return Status.Failure;

        runDirection = Mathf.Sign(Target.Value.transform.position.x - Agent.Value.transform.position.x);
        if (runDirection == 0) runDirection = 1;

        Vector3 scale = Agent.Value.transform.localScale;
        scale.x = runDirection > 0 ? -1 : 1;
        Agent.Value.transform.localScale = scale;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || rb == null)
            return Status.Failure;

        Vector2 position = rb.position + new Vector2(runDirection * Speed.Value * Time.fixedDeltaTime, 0f);
        rb.MovePosition(position);

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}
