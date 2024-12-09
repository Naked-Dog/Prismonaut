namespace PlayerSystem
{
    public abstract class PlayerMovement
    {
        protected EventBus eventBus;
        protected PlayerMovementScriptable movementValues;

        protected PlayerMovement(EventBus eventBus, PlayerMovementScriptable movementValues)
        {
            this.eventBus = eventBus;
            this.movementValues = movementValues;
        }

        public abstract void MoveHorizontally(HorizontalInputEvent e);
        public abstract void Jump(JumpInputEvent e);
        public abstract void UpdateGravity(UpdateEvent e);
    }
}
