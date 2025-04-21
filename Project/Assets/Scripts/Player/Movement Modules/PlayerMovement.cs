namespace PlayerSystem
{
    public abstract class PlayerMovement
    {
        protected EventBus eventBus;
        protected PlayerMovementScriptable movementConstants;
        protected bool isMovementDisabled = false;
        protected bool isJumpingDisabled = false;
        protected bool isGravityDisabled = false;

        protected PlayerMovement(EventBus eventBus, PlayerMovementScriptable movementConstants)
        {
            this.eventBus = eventBus;
            this.movementConstants = movementConstants;
        }

        protected abstract void OnHorizontalInput(OnHorizontalInput e);
        protected abstract void OnJumpInput(OnJumpInput e);
    }
}
