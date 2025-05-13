
using UnityEngine;

public enum SlimeSoundsEnum
{
    Slime,
    Bounce,
    Hit
}

[CreateAssetMenu(menuName = "Audio/Slime Sounds Library")]
public class SlimesLibrarySounds : AudioLibrary<SlimeSoundsEnum> {}
