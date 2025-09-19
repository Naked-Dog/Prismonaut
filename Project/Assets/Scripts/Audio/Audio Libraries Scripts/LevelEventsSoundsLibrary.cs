using UnityEngine;

public enum LevelEventsSoundsEnum
{
    Lava,
    Earthquake,
    EartThrumbling,
    Portal,
    BossStartZone,
}

[CreateAssetMenu(menuName = "Audio/Level Events Sounds Library")]
public class LevelEventsSoundsLibrary : AudioLibrary<LevelEventsSoundsEnum>{}
