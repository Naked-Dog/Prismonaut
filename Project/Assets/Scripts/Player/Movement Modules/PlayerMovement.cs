namespace PlayerSystem
{
    public abstract class PlayerMovement
    {
        protected EventBus eventBus;
        protected PlayerMovementScriptable movementConstants;
        protected bool isMovementDisabled = false;
        protected bool isJumpingDisabled = false;
        protected bool isGravityDisabled = false;

        protected PlayerMovement(EventBus eventBus)
        {
            this.eventBus = eventBus;
            this.movementConstants = GlobalConstants.Get<PlayerMovementScriptable>();
        }

        protected abstract void OnHorizontalInput(OnHorizontalInput e);
        protected abstract void OnJumpInput(OnJumpInput e);
    }
}
