namespace PlayerSystem
{
    public class PlayerState
    {
        public HealthState healthState = HealthState.Undefined;
        public GroundState groundState = GroundState.Undefined;
        public Power activePower = Power.None;
    }
}
