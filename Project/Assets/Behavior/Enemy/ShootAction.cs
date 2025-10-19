using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shoot", story: "[Scarabull] shoots [Shockwave] from [ShockWavePoint]", category: "Action", id: "6ef0af8afe6385a375768e934c3ed046")]
public partial class ShootAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Scarabull;
    [SerializeReference] public BlackboardVariable<GameObject> Shockwave;
    [SerializeReference] public BlackboardVariable<GameObject> ShockWavePoint;

    protected override Status OnStart()
    {
        if (Scarabull?.Value == null || Shockwave?.Value == null || ShockWavePoint?.Value == null)
            return Status.Failure;

        var prefab = Shockwave.Value;
        var spawnPoint = ShockWavePoint.Value.transform;

        GameObject instance = UnityEngine.Object.Instantiate(
            prefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        instance.transform.SetParent(Scarabull.Value.transform, true);

        return Status.Success;
    }
}

