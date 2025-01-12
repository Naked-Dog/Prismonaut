using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    // Use this class to gatekeep powers
    // If the player is not supposed to have a power, then don't instantiate it
    public class PlayerPowersModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private SquarePowerModule squarePower;
        private TrianglePowerModule trianglePower;
        private CirclePowerModule circlePower;
        private Rigidbody2D rb2d;
        private Dictionary<Direction, TriggerEventHandler> triggers;
        private Knockback knockback;
        private PlayerMovementScriptable movementValues;

        public PlayerPowersModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Dictionary<Direction, TriggerEventHandler> triggers, Knockback knockback, PlayerMovementScriptable movementValues)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.triggers = triggers;
            this.knockback = knockback;
            this.movementValues = movementValues;
            if (this.playerState.isSquarePowerAvailable) squarePower = new SquarePowerModule(eventBus, playerState, rb2d, triggers[Direction.Down], knockback, movementValues);
            if (this.playerState.isTrianglePowerAvailable) trianglePower = new TrianglePowerModule(eventBus, playerState, rb2d, triggers[Direction.Up], movementValues);
            if (this.playerState.isCirclePowerAvailable) circlePower = new CirclePowerModule(eventBus, playerState, rb2d, triggers[Direction.Left], triggers[Direction.Right], knockback, movementValues);
        }

        public void SetPowerAvailable(Power power, bool isAvailable)
        {
            switch (power)
            {
                case Power.Square:
                    playerState.isSquarePowerAvailable = isAvailable;
                    squarePower ??= new SquarePowerModule(eventBus, playerState, rb2d, triggers[Direction.Down], knockback, movementValues);
                    break;
                case Power.Triangle:
                    playerState.isTrianglePowerAvailable = isAvailable;
                    trianglePower ??= new TrianglePowerModule(eventBus, playerState, rb2d, triggers[Direction.Up], movementValues);
                    break;
                case Power.Circle:
                    playerState.isCirclePowerAvailable = isAvailable;
                    circlePower ??= new CirclePowerModule(eventBus, playerState, rb2d, triggers[Direction.Left], triggers[Direction.Right], knockback, movementValues);
                    break;
            }
        }
    }
}