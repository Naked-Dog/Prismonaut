using UnityEngine;

public enum EnemySpitterSoundsEnum
{
    Shoot,
    Idle,
    Death,
}

[CreateAssetMenu(menuName = "Audio/Spitter Sounds Library")]
public class EnemySpitterLibrary : AudioLibrary<EnemySpitterSoundsEnum>{}
