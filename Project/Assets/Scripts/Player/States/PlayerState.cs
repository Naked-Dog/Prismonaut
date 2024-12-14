namespace PlayerSystem
{
    public class PlayerState
    {
        public Direction facingDirection = Direction.Undefined;
        public HealthState healthState = HealthState.Undefined;
        public GroundState groundState = GroundState.Undefined;
        public Power activePower = Power.None;
    }
}
