using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public struct HorizontalInputEvent
    {
        public float amount;
        public HorizontalInputEvent(float amount) { this.amount = amount; }
    }
    public struct VerticalInputEvent
    {
        public float amount;
        public VerticalInputEvent(float amount) { this.amount = amount; }
    }
    public struct JumpInputEvent
    {
        public InputAction jumpInputAction;
        public JumpInputEvent(InputAction jumpInputAction) { this.jumpInputAction = jumpInputAction; }
    }
    public struct LookDownInputEvent
    {
        public bool toggle;
        public LookDownInputEvent(bool toggle) { this.toggle = toggle; }
    }
    public struct SquarePowerInputEvent
    {
        public bool toggle;
        public SquarePowerInputEvent(bool toggle) { this.toggle = toggle; }
    }
    public struct TrianglePowerInputEvent { }
    public struct CirclePowerInputEvent { }
    public struct PauseInputEvent { }
    public struct InteractionInputEvent { }
    public struct EnablePlayerInputsEvent { }
    public struct StopPlayerInputsEvent { }
    public struct EnableDialogueInputsEvent { }
    public struct DisableDilagueInputsEvent { }
}

