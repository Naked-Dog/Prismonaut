namespace PlayerSystem
{
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
    public struct PauseInputEvent { }
}

