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

    private float minForceMultiplier = 0.8f;
    private float maxForceMultiplier = 1.4f;

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

        float t = (float)spawned / (rockCount - 1);

        float amplitude = 25f;
        float baseAngle = -10f;
        float waveAngle = Mathf.Cos(t * Mathf.PI * 0.5f) * amplitude + baseAngle;

        float forceMultiplier = Mathf.Lerp(maxForceMultiplier, minForceMultiplier, t);

        RockProjectile projectile = instance.GetComponent<RockProjectile>();
        if (projectile != null)
        {
            projectile.SetLaunchAngle(waveAngle);
            projectile.SetForceMultiplier(forceMultiplier);
        }
    }
}

