namespace PlayerSystem
{
    // General Events
    public struct UpdateEvent { }
    public struct FixedUpdateEvent { }

    // Movement Events
    public struct HorizontalMovementEvent
    {
        public float amount;
        public HorizontalMovementEvent(float amount) { this.amount = amount; }
    }
    public struct JumpMovementEvent { }
    public struct UngroundedMovementEvent { }
    public struct GroundedMovementEvent { }


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
}