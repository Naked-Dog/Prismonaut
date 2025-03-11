namespace PlayerSystem
{
    public struct HorizontalMovementEvent
    {
        public float amount;
        public HorizontalMovementEvent(float amount) { this.amount = amount; }
    }
    public struct OnJumpMovement { }
    public struct OnUngroundedMovement { }
    public struct OnGroundedMovement { }
}