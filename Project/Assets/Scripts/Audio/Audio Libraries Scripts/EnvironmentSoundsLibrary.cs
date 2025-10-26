using UnityEngine;

public enum EnvironmentSoundsEnum
{
    Wind,
}

[CreateAssetMenu(menuName = "Audio/Environment Sounds Library")]
public class EnvironmentSoundsLibrary : AudioLibrary<EnvironmentSoundsEnum> { }