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

        public PlayerPowersModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Dictionary<Direction, TriggerEventHandler> triggers, Knockback knockback)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.triggers = triggers;
            this.knockback = knockback;
            if (this.playerState.isSquarePowerAvailable) squarePower = new SquarePowerModule(eventBus, playerState, rb2d, triggers[Direction.Down], knockback);
            if (this.playerState.isTrianglePowerAvailable) trianglePower = new TrianglePowerModule(eventBus, playerState, rb2d, triggers[Direction.Up]);
            if (this.playerState.isCirclePowerAvailable) circlePower = new CirclePowerModule(eventBus, playerState, rb2d, triggers[Direction.Left], triggers[Direction.Right]);
        }

        public void SetPowerAvailable(Power power, bool isAvailable)
        {
            Debug.Log("Setting " + power + " to " + isAvailable);
            switch (power)
            {
                case Power.Square:
                    playerState.isSquarePowerAvailable = isAvailable;
                    Debug.Log("is squarePower null" + (squarePower == null));
                    squarePower ??= new SquarePowerModule(eventBus, playerState, rb2d, triggers[Direction.Down], knockback);
                    break;
                case Power.Triangle:
                    playerState.isTrianglePowerAvailable = isAvailable;
                    Debug.Log("is trianglePower null" + (trianglePower == null));
                    trianglePower ??= new TrianglePowerModule(eventBus, playerState, rb2d, triggers[Direction.Up]);
                    break;
                case Power.Circle:
                    Debug.Log("is circlePower null" + (circlePower == null));
                    playerState.isCirclePowerAvailable = isAvailable;
                    circlePower ??= new CirclePowerModule(eventBus, playerState, rb2d, triggers[Direction.Left], triggers[Direction.Right]);
                    break;
            }
        }
    }
}