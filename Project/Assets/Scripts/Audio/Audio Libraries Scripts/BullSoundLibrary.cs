using UnityEngine;

public enum BullSoundsEnum
{
    Death,
    Detected,
    Hurt,
    LoopRush,
    RushCharge,
    Step1,
    Step2,
    Step3,
    RockAttackPrep,
    RockAttack,
    RockOnGround,
    AirAttackPrep,
    AirAttack,
    BossFinishZone,
}

[CreateAssetMenu(menuName = "Audio/Bull Sounds Library")]
public class BullSoundLibrary : AudioLibrary<BullSoundsEnum> { }
