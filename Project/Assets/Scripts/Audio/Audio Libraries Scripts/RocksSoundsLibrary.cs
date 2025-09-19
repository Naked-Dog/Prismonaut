using UnityEngine;

public enum RocksSounds
{
    PrepareThrow,
    ThrowRock,
    Destroy
}

[CreateAssetMenu(menuName = "Audio/Rocks Sounds Library")]
public class RocksSoundsLibrary : AudioLibrary<RocksSounds> {}