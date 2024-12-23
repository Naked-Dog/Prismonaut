using UnityEngine;

namespace PlayerSystem
{
    public class HardKeyboardInput : PlayerInput
    {
        public HardKeyboardInput(EventBus eventBus) : base(eventBus)
        {
            eventBus.Subscribe<UpdateEvent>(GetInput);
        }

        public override void GetInput(UpdateEvent e)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            eventBus.Publish(new HorizontalInputEvent(horizontalInput));

            if (Input.GetButtonDown("Jump")) eventBus.Publish(new JumpInputEvent());

            if (Input.GetKeyDown(KeyCode.J)) eventBus.Publish(new SquarePowerInputEvent(true));
            if (Input.GetKeyUp(KeyCode.J)) eventBus.Publish(new SquarePowerInputEvent(false));
            if (Input.GetKeyDown(KeyCode.K)) eventBus.Publish(new TrianglePowerInputEvent());
            if (Input.GetKeyDown(KeyCode.L)) eventBus.Publish(new CirclePowerInputEvent());
        }
    }

}