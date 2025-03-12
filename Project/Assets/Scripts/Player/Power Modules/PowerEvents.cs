namespace PlayerSystem
{
    public class ToggleSquarePowerEvent
    {
        public bool toggle;
        public ToggleSquarePowerEvent(bool toggle) { this.toggle = toggle; }
    }
    public class ToggleTrianglePowerEvent
    {
        public bool toggle;
        public ToggleTrianglePowerEvent(bool toggle) { this.toggle = toggle; }
    }
    public class ToggleCirclePowerEvent
    {
        public bool toggle;
        public ToggleCirclePowerEvent(bool toggle) { this.toggle = toggle; }
    }

    public class OnPowerActivation { }
    public class OnDodgeActivation : OnPowerActivation { }
    public class OnShieldActivation : OnPowerActivation { }

    public class OnPowerDeactivation { }
    public class OnDodgeDeactivation : OnPowerDeactivation { }
    public class OnShieldDeactivation : OnPowerDeactivation { }
}