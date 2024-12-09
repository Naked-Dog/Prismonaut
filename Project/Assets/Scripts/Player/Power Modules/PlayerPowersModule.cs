namespace PlayerSystem
{
    public class PlayerPowersModule
    {
        private EventBus eventBus;
        private SquarePowerModule squarePower;
        private TrianglePowerModule trianglePower;
        private CirclePowerModule circlePower;

        public PlayerPowersModule(EventBus eventBus)
        {
            this.eventBus = eventBus;
            squarePower = new SquarePowerModule(eventBus);
            trianglePower = new TrianglePowerModule(eventBus);
            circlePower = new CirclePowerModule(eventBus);

            eventBus.Subscribe<SquarePowerInputEvent>(UseSquarePower);
            eventBus.Subscribe<TrianglePowerInputEvent>(UseTrianglePower);
            eventBus.Subscribe<CirclePowerInputEvent>(UseCirclePower);
        }

        private void UseSquarePower(SquarePowerInputEvent e)
        {
            eventBus.Publish(new UseSquarePowerEvent());
        }

        private void UseTrianglePower(TrianglePowerInputEvent e)
        {
            eventBus.Publish(new UseTrianglePowerEvent());
        }

        private void UseCirclePower(CirclePowerInputEvent e)
        {
            eventBus.Publish(new UseCirclePowerEvent());
        }
    }
}