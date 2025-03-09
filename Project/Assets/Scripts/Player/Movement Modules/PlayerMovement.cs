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

        protected abstract void MoveHorizontally(HorizontalInputEvent e);
        protected abstract void OnJumpInput(JumpInputEvent e);
    }
}
