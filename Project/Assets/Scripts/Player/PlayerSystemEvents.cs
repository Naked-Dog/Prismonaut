namespace PlayerSystem
{
    // General Events
    public struct UpdateEvent { }
    public struct FixedUpdateEvent { }


    // Input Events
    public struct HorizontalInputEvent
    {
        public float amount;
        public HorizontalInputEvent(float amount) { this.amount = amount; }
    }
    public struct JumpInputEvent { }
    public struct SquarePowerInputEvent
    {
        public bool toggle;
        public SquarePowerInputEvent(bool toggle) { this.toggle = toggle; }
    }
    public struct TrianglePowerInputEvent { }
    public struct CirclePowerInputEvent { }


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
        public Direction direction;
        public ToggleCirclePowerEvent(bool toggle, Direction direction) { this.toggle = toggle; this.direction = direction; }
    }
}