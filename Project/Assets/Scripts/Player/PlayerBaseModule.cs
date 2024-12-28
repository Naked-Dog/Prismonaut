using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public class PlayerBaseModule : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D avatarRigidbody2D;
        [SerializeField] private Animator spriteAnimator;
        [SerializeField] private TriggerEventHandler groundTrigger;
        [SerializeField] private TriggerEventHandler leftTrigger;
        [SerializeField] private TriggerEventHandler rightTrigger;
        [SerializeField] private TriggerEventHandler upTrigger;
        [SerializeField] private TriggerEventHandler downTrigger;
        [SerializeField] private PlayerMovementScriptable movementValues;
        [SerializeField] private InputActionAsset  playerInputAsset;

        private PlayerState state;
        private EventBus eventBus;
        private Dictionary<Direction, TriggerEventHandler> triggers;

        private PlayerInput inputModule;
        private PlayerMovement movementModule;
        private PlayerVisuals visualsModule;
        private PlayerPowersModule powersModule;

        protected void Start()
        {
            state = new PlayerState();
            eventBus = new EventBus();

            triggers = new Dictionary<Direction, TriggerEventHandler>() {
                {Direction.Up, upTrigger},
                {Direction.Down, downTrigger},
                {Direction.Left, leftTrigger},
                {Direction.Right, rightTrigger}
            };

            inputModule = new HardKeyboardInput(eventBus,playerInputAsset);
            movementModule = new Tight2DMovement(eventBus, state, movementValues, avatarRigidbody2D, groundTrigger);
            visualsModule = new PlayerVisuals(eventBus, state, avatarRigidbody2D, spriteAnimator);
            powersModule = new PlayerPowersModule(eventBus, state, avatarRigidbody2D, triggers);
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!state.isPaused)
                {
                    state.velocity = avatarRigidbody2D.velocity;
                    avatarRigidbody2D.velocity = Vector2.zero;
                }
                else
                {
                    avatarRigidbody2D.velocity = state.velocity;
                }
                state.isPaused = !state.isPaused;
            }
            if (state.isPaused) return;
            eventBus.Publish(new UpdateEvent());
        }

        protected void FixedUpdate()
        {
            if (state.isPaused) return;
            eventBus.Publish(new FixedUpdateEvent());
        }
    }
}
