using UnityEngine;

namespace PlayerSystem
{
    public class PlayerState
    {
        public bool isPaused = false;
        public Vector2 velocity = Vector2.zero;
        public float rotation = 0f;
        public Direction facingDirection = Direction.Right;
        public HealthState healthState = HealthState.Undefined;
        public GroundState groundState = GroundState.Undefined;
        public Power activePower = Power.None;
        public Vector2 lastSafeGroundLocation = Vector2.zero;
        public bool isSquarePowerAvailable = false;
        public bool isTrianglePowerAvailable = false;
        public bool isCirclePowerAvailable = true;
        public Power currentPower = Power.Circle;
        public float powerTimeLeft = 0f;
        public bool isOnInteractable = false;
        public bool isParry;

        // Health
        public int maxHealthBars = 3;
        public int currentHealthBars = 3;
        public int healthPerBar = 8;
        public int currentHealth = 8;
        public float hpRegenRate = 2f;
        
        // Power Charges
        public int maxCharges = 1;
        public float currentCharges = 1f;
        public float chargeCooldown = 2f;
        public bool isRecharging = false;
    }
}
