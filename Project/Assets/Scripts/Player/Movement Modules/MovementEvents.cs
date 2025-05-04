namespace PlayerSystem
{
    public struct OnHorizontalMovement
    {
        public float amount;
        public OnHorizontalMovement(float amount) { this.amount = amount; }
    }
    public struct OnJumpMovement { }
    public struct OnUngroundedMovement { }
    public struct OnGroundedMovement { }
    public struct RequestMovementPause { }
    public struct RequestMovementResume { }
    public struct RequestGravityOff { }
    public struct RequestGravityOn { }
    public struct RequestOppositeReaction{ }
}