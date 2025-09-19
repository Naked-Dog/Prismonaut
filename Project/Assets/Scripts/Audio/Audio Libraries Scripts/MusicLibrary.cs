using UnityEngine;

public enum MusicEnum
{
    Menu,
    Level1,
    BossRoom,
    None,
}

[CreateAssetMenu(menuName = "Audio/Music Library")]
public class MusicLibrary : AudioLibrary<MusicEnum>{}
