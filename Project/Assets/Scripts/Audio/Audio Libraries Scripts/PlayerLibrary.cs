using UnityEngine;

public enum PlayerSoundsEnum
{
    Step1,
    Step2,
    Step3,
    Step4,
    Explode,
    Defeat,
    Jump,
    Interact,
    DodgeTrans,
    ShieldTrans,
    DrillTrans,
    DrillDig,
    Parry,
    HeavyLand,
    LoopWindFall,
    Heal,
    Land,
    Hurt,
    RegularForm,
}

[CreateAssetMenu(menuName = "Audio/Player Sounds Library")]
public class PlayerSoundsLibrary : AudioLibrary<PlayerSoundsEnum> { }