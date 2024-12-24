namespace PlayerSystem
{
    public abstract class PlayerMovement
    {
        protected EventBus eventBus;
        protected PlayerMovementScriptable movementValues;
        protected bool isMovementDisabled = false;
        protected bool isJumpingDisabled = false;
        protected bool isGravityDisabled = false;

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
