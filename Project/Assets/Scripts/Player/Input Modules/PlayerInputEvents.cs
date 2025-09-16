using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public struct OnHorizontalInput
    {
        public float amount;
        public OnHorizontalInput(float amount) { this.amount = amount; }
    }
    public struct OnVerticalInput
    {
        public float amount;
        public OnVerticalInput(float amount) { this.amount = amount; }
    }
    public struct OnJumpInput
    {
        public InputAction.CallbackContext context;
        public OnJumpInput(InputAction.CallbackContext context) { this.context = context; }
    }
    public struct OnLookDownInput
    {
        public bool toggle;
        public OnLookDownInput(bool toggle) { this.toggle = toggle; }
    }
    public struct OnLookUpInput
    {
        public bool toggle;
        public OnLookUpInput(bool toggle) { this.toggle = toggle; }
    }
    public struct OnSquarePowerInput
    {
        public InputAction.CallbackContext context;
        public OnSquarePowerInput(InputAction.CallbackContext context) { this.context = context; }        
    }
    public struct OnTrianglePowerInput { }
    public struct OnCirclePowerInput { }
    public struct OnPauseInput { }
    public struct OnInteractionInput { }
    public struct RequestPlayerInputs { }
    public struct RequestStopPlayerInputs { }
    public struct RequestEnableDialogueInputs { }
    public struct RequestDisableDialogueInputs { }
}

