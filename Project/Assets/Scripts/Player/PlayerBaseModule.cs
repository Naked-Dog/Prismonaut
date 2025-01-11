using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public class PlayerBaseModule : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D avatarRigidbody2D;
        [SerializeField] private Animator spriteAnimator;
        [SerializeField] private SpriteRenderer helmetRenderer;
        [SerializeField] private TriggerEventHandler groundTrigger;
        [SerializeField] private TriggerEventHandler leftTrigger;
        [SerializeField] private TriggerEventHandler rightTrigger;
        [SerializeField] private TriggerEventHandler upTrigger;
        [SerializeField] private TriggerEventHandler downTrigger;
        [SerializeField] private PlayerMovementScriptable movementValues;
        [SerializeField] private InputActionAsset playerInputAsset;
        [SerializeField] private Knockback knockback;
        [SerializeField] private HealthUIController healthUIController;

        private PlayerState state;
        private EventBus eventBus;
        private Dictionary<Direction, TriggerEventHandler> triggers;

        private PlayerInput inputModule;
        private PlayerMovement movementModule;
        private PlayerVisuals visualsModule;
        public PlayerPowersModule powersModule;
        public PlayerHealthModule healthModule;
        private PlayerAudioModule audioModule;
        

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

            audioModule = new PlayerAudioModule(eventBus, GetComponent<PlayerSounds>(), gameObject, GetComponent<AudioSource>());
            inputModule = new PlayerInput(eventBus, playerInputAsset);
            movementModule = new Tight2DMovement(eventBus, state, movementValues, avatarRigidbody2D, groundTrigger,  audioModule, this);
            visualsModule = new PlayerVisuals(eventBus, state, avatarRigidbody2D, spriteAnimator, helmetRenderer);
            powersModule = new PlayerPowersModule(eventBus, state, avatarRigidbody2D, triggers, knockback, movementValues);
            healthModule = new PlayerHealthModule(eventBus, state, avatarRigidbody2D, knockback, healthUIController, this)
            {
                MaxHealth = 3
            };

            healthModule.CurrentHealth = healthModule.MaxHealth;
            healthUIController.InitUI(healthModule.CurrentHealth);
            MenuController.Instance?.setEvents(eventBus);
            DialogueController.Instance?.SetEventBus(eventBus);
            spriteAnimator.GetComponent<PlayerAnimationEvents>()?.SetEventBus(eventBus);

            GameDataManager.Instance.SavePlayerPosition(avatarRigidbody2D.position);
        }

        protected void Update()
        {
            eventBus.Publish(new UpdateEvent());
        }

        protected void FixedUpdate()
        {
            eventBus.Publish(new FixedUpdateEvent());
        }

        protected void LateUpdate()
        {
            eventBus.Publish(new LateUpdateEvent());
        }
    }
}
