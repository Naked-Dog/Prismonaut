using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public class HardKeyboardInput : PlayerInput
    {
        private InputActionAsset playerInputAsset;
        private InputActionMap playerMap =>  playerInputAsset.FindActionMap("Player");
        private InputActionMap uiMap => playerInputAsset.FindActionMap("UI"); 
        private InputAction moveInput => playerMap.FindAction("Move");
        private InputAction jumpInput => playerMap.FindAction("Jump");
        private InputAction trianglePowerInput => playerMap.FindAction("TrianglePower");
        private InputAction circlePowerInput => playerMap.FindAction("CirclePower");
        private InputAction squarePowerInput => playerMap.FindAction("SquarePower");
        private InputAction pauseInput => playerMap.FindAction("Pause");

        private bool actionsEnable = true;
        public HardKeyboardInput(EventBus eventBus, InputActionAsset playerInputAsset) : base(eventBus)
        {
            this.playerInputAsset = playerInputAsset;
            EnablePlayerActions();

            eventBus.Subscribe<UpdateEvent>(GetInput);
        }

        public override void GetInput(UpdateEvent e)
        {
            float horizontalInput = moveInput.ReadValue<Vector2>().x;
            eventBus.Publish(new HorizontalInputEvent(horizontalInput));

            if (jumpInput.WasPressedThisFrame()) eventBus.Publish(new JumpInputEvent());
            if (squarePowerInput.WasPressedThisFrame()) eventBus.Publish(new SquarePowerInputEvent(true));
            if (squarePowerInput.WasReleasedThisFrame()) eventBus.Publish(new SquarePowerInputEvent(false));
            if (trianglePowerInput.WasPressedThisFrame()) eventBus.Publish(new TrianglePowerInputEvent());
            if (circlePowerInput.WasPressedThisFrame()) eventBus.Publish(new CirclePowerInputEvent());
            if (pauseInput.WasPressedThisFrame()){
                actionsEnable = !actionsEnable;
                EnablePlayerActions(actionsEnable);
                MenuController.Instance.DisplayGamePanel();
            };
        }

        private void EnablePlayerActions(bool enabled = true)
        {
            foreach(InputAction action in playerMap.actions)
            {
                if(action == pauseInput) continue;

                if(enabled)
                {
                    action.Enable();
                }
                else
                {
                    action.Disable();
                }
            }
        }
    }

}