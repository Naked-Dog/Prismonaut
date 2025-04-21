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
        private TriggerEventHandler drillTrigger;
        private HingeJoint2D drillJoint;
        private Dictionary<Direction, TriggerEventHandler> triggers;
        private PlayerPowersScriptable powersConstants;

        public PlayerPowersModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            TriggerEventHandler drillTrigger,
            HingeJoint2D drillJoint,
            PlayerPowersScriptable powersConstants)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.drillTrigger = drillTrigger;
            this.drillJoint = drillJoint;
            this.powersConstants = powersConstants;

            squarePower = new ShieldPowerModule(eventBus, playerState, rb2d, powersConstants);
            trianglePower = new DrillPowerModule(eventBus, playerState, rb2d, drillTrigger, drillJoint, powersConstants);
            circlePower = new DodgePowerModule(eventBus, playerState, rb2d, powersConstants);
        }

        public void SetPowerAvailable(Power power, bool isAvailable)
        {
            switch (power)
            {
                case Power.Square:
                    playerState.isSquarePowerAvailable = isAvailable;
                    squarePower ??= new ShieldPowerModule(eventBus, playerState, rb2d, powersConstants);
                    break;
                case Power.Triangle:
                    playerState.isTrianglePowerAvailable = isAvailable;
                    trianglePower ??= new DrillPowerModule(eventBus, playerState, rb2d, drillTrigger, drillJoint, powersConstants);
                    break;
                case Power.Circle:
                    playerState.isCirclePowerAvailable = isAvailable;
                    circlePower ??= new DodgePowerModule(eventBus, playerState, rb2d, powersConstants);
                    break;
            }
        }
    }
}