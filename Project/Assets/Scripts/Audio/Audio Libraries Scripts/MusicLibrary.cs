using UnityEngine;

public enum MusicEnum
{
    Menu,
    Level1,
    BossRoom,
    FinalCinematic,
    None,
}

[CreateAssetMenu(menuName = "Audio/Music Library")]
public class MusicLibrary : AudioLibrary<MusicEnum>{}
