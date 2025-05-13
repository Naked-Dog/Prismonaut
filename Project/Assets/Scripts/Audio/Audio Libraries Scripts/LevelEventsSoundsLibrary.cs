using UnityEngine;

public enum LevelEventsSoundsEnum
{
    Lava,
    Earthquake,
    EartThrumbling,
    Portal,
}

[CreateAssetMenu(menuName = "Audio/Level Events Sounds Library")]
public class LevelEventsSoundsLibrary : AudioLibrary<LevelEventsSoundsEnum>{}
