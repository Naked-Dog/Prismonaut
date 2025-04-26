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
        private ShieldPowerModule squarePower;
        private DrillPowerModule trianglePower;
        private DodgePowerModule circlePower;
        private Rigidbody2D rb2d;
        private PhysicsEventsRelay drillPhysicsRelay;
        private HingeJoint2D drillJoint;

        public PlayerPowersModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            PhysicsEventsRelay drillPhysicsRelay,
            HingeJoint2D drillJoint)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.drillPhysicsRelay = drillPhysicsRelay;
            this.drillJoint = drillJoint;

            squarePower = new ShieldPowerModule(eventBus, playerState, rb2d);
            trianglePower = new DrillPowerModule(eventBus, playerState, rb2d, drillPhysicsRelay, drillJoint);
            circlePower = new DodgePowerModule(eventBus, playerState, rb2d);
        }

        public void SetPowerAvailable(Power power, bool isAvailable)
        {
            switch (power)
            {
                case Power.Square:
                    playerState.isSquarePowerAvailable = isAvailable;
                    squarePower ??= new ShieldPowerModule(eventBus, playerState, rb2d);
                    break;
                case Power.Triangle:
                    playerState.isTrianglePowerAvailable = isAvailable;
                    trianglePower ??= new DrillPowerModule(eventBus, playerState, rb2d, drillPhysicsRelay, drillJoint);
                    break;
                case Power.Circle:
                    playerState.isCirclePowerAvailable = isAvailable;
                    circlePower ??= new DodgePowerModule(eventBus, playerState, rb2d);
                    break;
            }
        }
    }
}