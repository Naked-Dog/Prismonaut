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
        public int maxHealth = 3;
        public int maxCharges = 1;
        public int currentCharges = 1;
        public float chargeCooldown = 2f;
        public bool isRecharging = false;
    }
}
