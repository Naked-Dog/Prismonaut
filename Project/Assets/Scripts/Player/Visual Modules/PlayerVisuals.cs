namespace PlayerSystem
{
    public abstract class PlayerVisuals
    {
        protected EventBus eventBus;

        protected PlayerVisuals(EventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        protected abstract void MoveHorizontally(HorizontalMovementEvent e);
        protected abstract void Jump(JumpMovementEvent e);
        protected abstract void Unground(UngroundedMovementEvent e);
        protected abstract void Ground(GroundedMovementEvent e);
    }
}