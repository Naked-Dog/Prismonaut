using UnityEngine;

public enum ThornRootSoundsEnum
{
    Charge,
    PrevAttackGlow,
    StartAttack,
    EndAttack,
    Break,
    Recover,
}

[CreateAssetMenu(menuName = "Audio/Thorn Root Sounds Library")]
public class ThornRootSoundsLibrary : AudioLibrary<ThornRootSoundsEnum> { }