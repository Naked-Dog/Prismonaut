using UnityEngine;

namespace PlayerSystem
{
    public class PlayerPowersModule
    {
        private EventBus eventBus;
        private SquarePowerModule squarePower;
        private TrianglePowerModule trianglePower;
        private CirclePowerModule circlePower;

        private bool isSquarePowerAvailable = true;
        private bool isTrianglePowerAvailable = true;
        private bool isCirclePowerAvailable = true;

        public PlayerPowersModule(EventBus eventBus, Rigidbody2D rb2d)
        {
            this.eventBus = eventBus;
            squarePower = new SquarePowerModule(eventBus, rb2d);
            trianglePower = new TrianglePowerModule(eventBus);
            circlePower = new CirclePowerModule(eventBus);

            eventBus.Subscribe<SquarePowerInputEvent>(UseSquarePower);
            eventBus.Subscribe<TrianglePowerInputEvent>(UseTrianglePower);
            eventBus.Subscribe<CirclePowerInputEvent>(UseCirclePower);
        }

        private void UseSquarePower(SquarePowerInputEvent e)
        {
            if (isSquarePowerAvailable) squarePower.togglePower(e.toggle);
        }

        private void UseTrianglePower(TrianglePowerInputEvent e)
        {
            if (isTrianglePowerAvailable) eventBus.Publish(new UseTrianglePowerEvent());
        }

        private void UseCirclePower(CirclePowerInputEvent e)
        {
            if (isCirclePowerAvailable) eventBus.Publish(new UseCirclePowerEvent());
        }
    }
}