namespace PlayerSystem
{
    public abstract class PlayerInput
    {
        protected EventBus eventBus;

        protected PlayerInput(EventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public abstract void GetInput(UpdateEvent e);
    }
}

