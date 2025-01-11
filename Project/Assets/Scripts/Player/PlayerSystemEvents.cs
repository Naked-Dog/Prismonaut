using UnityEngine;

namespace PlayerSystem
{
    // General Events
    public struct UpdateEvent { }
    public struct FixedUpdateEvent { }
    public struct LateUpdateEvent { }

    // Movement Events
    public struct HorizontalMovementEvent
    {
        public float amount;
        public HorizontalMovementEvent(float amount) { this.amount = amount; }
    }
    public struct JumpMovementEvent { }
    public struct UngroundedMovementEvent { }
    public struct GroundedMovementEvent { }
    public struct PauseEvent{ }
    public struct UnpauseEvent{ }
    public struct ReceivedDamageEvent{ }
    public struct DeathEvent{ }
    public struct RespawnEvent{ }

    // Power Events
    public struct ToggleSquarePowerEvent
    {
        public bool toggle;
        public ToggleSquarePowerEvent(bool toggle) { this.toggle = toggle; }
    }
    public struct ToggleTrianglePowerEvent
    {
        public bool toggle;
        public ToggleTrianglePowerEvent(bool toggle) { this.toggle = toggle; }
    }
    public struct ToggleCirclePowerEvent
    {
        public bool toggle;
        public ToggleCirclePowerEvent(bool toggle) { this.toggle = toggle; }
    }

    public struct PlayPlayerSounEffect
    {
        public string clipName;
        public float volume;
        public PlayPlayerSounEffect (string clipName, float volume = 1 )
        { 
            this.clipName = clipName;
            this.volume = volume; 
        }
    }
}