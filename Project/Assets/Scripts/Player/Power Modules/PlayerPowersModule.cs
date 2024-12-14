using System.Collections.Generic;
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

        public PlayerPowersModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Dictionary<Direction, TriggerEventHandler> triggers)
        {
            this.eventBus = eventBus;
            squarePower = new SquarePowerModule(eventBus, rb2d, triggers[Direction.Down]);
            trianglePower = new TrianglePowerModule(eventBus, triggers[Direction.Up]);
            circlePower = new CirclePowerModule(eventBus, rb2d, triggers[Direction.Left], triggers[Direction.Right]);

            eventBus.Subscribe<SquarePowerInputEvent>(UseSquarePower);
            eventBus.Subscribe<TrianglePowerInputEvent>(UseTrianglePower);
            eventBus.Subscribe<CirclePowerInputEvent>(UseCirclePower);
        }

        private void UseSquarePower(SquarePowerInputEvent e)
        {
            if (!isSquarePowerAvailable && e.toggle) return;
            squarePower.togglePower(e.toggle);
        }

        private void UseTrianglePower(TrianglePowerInputEvent e)
        {
            if (!isTrianglePowerAvailable) return;
            eventBus.Publish(new ToggleTrianglePowerEvent());
        }

        private void UseCirclePower(CirclePowerInputEvent e)
        {
            if (!isCirclePowerAvailable) return;
            eventBus.Publish(new ToggleCirclePowerEvent());
        }
    }
}