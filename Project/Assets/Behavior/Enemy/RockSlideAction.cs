using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RockSlide", story: "[Scarabull] throws [Rock] from [RockSlideSpawnpoint]", category: "Action", id: "e379e884809e8588aabb9e8c2bcf1f3d")]
public partial class RockSlideAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Scarabull;
    [SerializeReference] public BlackboardVariable<GameObject> Rock;
    [SerializeReference] public BlackboardVariable<GameObject> RockSlideSpawnpoint;

    private int rockCount = 5;
    private float interval = 0.1f;
    private int spawned;
    private float timer;

    protected override Status OnStart()
    {
        if (Scarabull?.Value == null || Rock?.Value == null || RockSlideSpawnpoint?.Value == null)
            return Status.Failure;

        spawned = 0;
        timer = 0f;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (spawned >= rockCount)
            return Status.Success;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            SpawnRock();
            spawned++;
        }

        return Status.Running;
    }

    private void SpawnRock()
    {
        var prefab = Rock.Value;
        var spawnPoint = RockSlideSpawnpoint.Value.transform;

        GameObject instance = UnityEngine.Object.Instantiate(
            prefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        instance.transform.SetParent(Scarabull.Value.transform, true);
    }
}

