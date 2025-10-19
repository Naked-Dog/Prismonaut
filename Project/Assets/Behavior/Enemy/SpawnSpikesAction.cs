using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SpawnSpikes", story: "Spawn [Spikes]", category: "Action", id: "d41e03f3d8d334a3ac6607cb4e33d146")]
public partial class SpawnSpikesAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Spikes;

    protected override Status OnStart()
    {
        Spikes.Value?.ForEach(spike =>
        {
            if (spike != null)
            {
                spike.GetComponent<SpikeSpawner>()?.TriggerSpawn();
            }
        });
        return Status.Success;
    }
}

