using UnityEngine;

namespace PlayerSystem
{
    public class PlayerBaseModule : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D avatarRigidbody2D;
        [SerializeField] private Animator spriteAnimator;
        [SerializeField] private TriggerEventHandler groundTrigger;
        [SerializeField] private PlayerMovementScriptable movementValues;

        private PlayerState state;
        private EventBus eventBus;

        private PlayerInput inputModule;
        private PlayerMovement movementModule;
        private PlayerVisuals visualsModule;
        private PlayerPowersModule powersModule;

        protected void Start()
        {
            state = new PlayerState();
            eventBus = new EventBus();

            inputModule = new HardKeyboardInput(eventBus);
            movementModule = new Tight2DMovement(eventBus, movementValues, this, state, avatarRigidbody2D, groundTrigger);
            visualsModule = new PlayerVisuals(eventBus, state, avatarRigidbody2D, spriteAnimator);
            powersModule = new PlayerPowersModule(eventBus, avatarRigidbody2D);
        }

        protected void Update()
        {
            eventBus.Publish(new UpdateEvent());
        }

        protected void FixedUpdate()
        {
            eventBus.Publish(new FixedUpdateEvent());
        }
    }
}
