using System.Collections.Generic;
using CameraSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public class PlayerBaseModule : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D avatarRigidbody2D;
        [SerializeField] private Animator animator;
        [SerializeField] private PhysicsEventsRelay drillPhysicsRelay;
        [SerializeField] private PhysicsEventsRelay drillExitPhysicsRelay;
        [SerializeField] private FixedJoint2D drillJoint;
        [SerializeField] private PhysicsEventsRelay shieldPhysicsRelay;
        [SerializeField] private InputActionAsset playerInputAsset;
        [SerializeField] private HealthUIController healthUIController;
        [SerializeField] private GameObject interactSign;

        public Knockback knockback;
        public PlayerState state;
        private EventBus eventBus;
        private Dictionary<Direction, TriggerEventHandler> triggers;

        private PlayerInput inputModule;
        private PlayerMovement movementModule;
        public PlayerAnimations animationsModule;
        public PlayerPowersModule powersModule;
        public PlayerHealthModule healthModule;
        private PlayerAudioModule audioModule;
        public PlayerInteractionModule interactionModule;


        protected void Start()
        {
            state = new PlayerState();
            eventBus = new EventBus();

            inputModule = new PlayerInput(eventBus, playerInputAsset);
            movementModule = new Physics2DMovement(eventBus, state, avatarRigidbody2D);
            animationsModule = new PlayerAnimations(eventBus, state, animator);
            powersModule = new PlayerPowersModule(eventBus, state, avatarRigidbody2D, drillPhysicsRelay, drillExitPhysicsRelay, drillJoint, shieldPhysicsRelay);
            healthModule = new PlayerHealthModule(eventBus, state, avatarRigidbody2D, healthUIController, this)
            {
                MaxHealth = 3
            };
            interactionModule = new PlayerInteractionModule(eventBus, gameObject.GetComponent<PhysicsEventsRelay>(), interactSign, state);

            avatarRigidbody2D.GetComponent<PhysicsEventsRelay>()?.OnCollisionEnter2DAction.AddListener(OnCollisionEnter2D);
            avatarRigidbody2D.GetComponent<PhysicsEventsRelay>()?.OnCollisionStay2DAction.AddListener(OnCollisionStay2D);
            avatarRigidbody2D.GetComponent<PhysicsEventsRelay>()?.OnCollisionExit2DAction.AddListener(OnCollisionExit2D);

            healthModule.CurrentHealth = healthModule.MaxHealth;
            healthUIController.InitUI(healthModule.CurrentHealth);
            MenuController.Instance?.setEvents(eventBus);
            DialogueController.Instance?.SetEventBus(eventBus);
            animator.GetComponent<PlayerAnimationEvents>()?.SetEventBus(eventBus);

            GameDataManager.Instance?.SavePlayerPosition(avatarRigidbody2D.position);
        }

        protected void Update()
        {
            eventBus.Publish(new OnUpdate());
        }

        protected void FixedUpdate()
        {
            eventBus.Publish(new OnFixedUpdate());
        }

        protected void LateUpdate()
        {
            eventBus.Publish(new OnLateUpdate());
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            eventBus.Publish(new OnCollisionEnter2D(collision));
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            eventBus.Publish(new OnCollisionStay2D(collision));
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            eventBus.Publish(new OnCollisionExit2D(collision));
        }

        private void OnDestroy()
        {
            inputModule.Dispose();
        }
    }
}
