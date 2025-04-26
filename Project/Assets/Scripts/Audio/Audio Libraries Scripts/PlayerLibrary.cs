using UnityEngine;

public enum PlayerSoundsEnum
{
    Walk,
    Die,
    Jump,
    Interact
}

[CreateAssetMenu(menuName = "Audio/Player Sounds Library")]
public class PlayerSoundsLibrary : AudioLibrary<PlayerSoundsEnum> { }