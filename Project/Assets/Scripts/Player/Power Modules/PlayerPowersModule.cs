using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    public class PlayerPowersModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private SquarePowerModule squarePower;
        private TrianglePowerModule trianglePower;
        private CirclePowerModule circlePower;

        private bool isSquarePowerAvailable = true;
        private bool isTrianglePowerAvailable = true;
        private bool isCirclePowerAvailable = true;

        public PlayerPowersModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Dictionary<Direction, TriggerEventHandler> triggers)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            squarePower = new SquarePowerModule(eventBus, rb2d, triggers[Direction.Down]);
            trianglePower = new TrianglePowerModule(eventBus, rb2d, triggers[Direction.Up]);
            circlePower = new CirclePowerModule(eventBus, playerState, rb2d, triggers[Direction.Left], triggers[Direction.Right]);
        }
    }
}