using System;
using Unity.Behavior;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlaySound", story: "Play Wall Impact Sound", category: "Action", id: "f1f0fb85f6c0c0bf8af0e48b09a4d70c")]
public partial class PlaySoundAction : Action
{

    protected override Status OnStart()
    {
        AudioManager.Instance?.Play2DSound(BullSoundsEnum.WallImpact);
        return Status.Success;
    }
}

