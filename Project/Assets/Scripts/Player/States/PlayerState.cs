using UnityEngine;

namespace PlayerSystem
{
    public class PlayerState
    {
        public bool isPaused = false;
        public Vector2 velocity = Vector2.zero;
        public Direction facingDirection = Direction.Undefined;
        public HealthState healthState = HealthState.Undefined;
        public GroundState groundState = GroundState.Undefined;
        public Power activePower = Power.None;
    }
}
