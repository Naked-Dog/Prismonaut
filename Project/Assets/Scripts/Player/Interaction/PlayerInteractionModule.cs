using UnityEngine;

namespace PlayerSystem
{
    public class PlayerInteractionModule
    {
        private EventBus eventBus;
        public IInteractable currentInteractable;
        private GameObject signObject;
        private PlayerState playerState;

        public PlayerInteractionModule(EventBus eventBus, TriggerEventHandler triggerEvent, GameObject interactSign, PlayerState state)
        {
            this.eventBus = eventBus;
            signObject = interactSign;
            playerState = state;
            signObject.SetActive(false);
            triggerEvent.OnTriggerEnter2DAction.AddListener(OnCollionEnter);
            triggerEvent.OnTriggerExit2DAction.AddListener(OnCollionExit);
            triggerEvent.OnTriggerStay2DAction.AddListener(OnTriggerStay);
            eventBus.Subscribe<InteractionInputEvent>(OnInteraction);
        }

        private void OnInteraction(InteractionInputEvent e)
        {
            if (currentInteractable == null) return;

            if (currentInteractable.IsInteractable)
            {
                AudioManager.Instance.Play(PlayerSoundsEnum.Interact);
                currentInteractable.Interact();
                playerState.isOnInteractable = false;
                signObject.SetActive(false);
            }
        }

        private void OnCollionEnter(Collider2D other)
        {
            var interactable = other.gameObject.GetComponent<IInteractable>();

            if (interactable != null)
            {
                playerState.isOnInteractable = true;
                currentInteractable = interactable;
                if (currentInteractable.InteractOnEnter)
                {
                    OnInteraction(new InteractionInputEvent());
                    return;
                }
                signObject.SetActive(currentInteractable.IsInteractable);
            }
        }
        private void OnCollionExit(Collider2D other)
        {
            var interactable = other.gameObject.GetComponent<IInteractable>();

            if (currentInteractable == interactable)
            {
                playerState.isOnInteractable = false;
                currentInteractable = null;
                signObject.SetActive(false);
            }

        }

        private void OnTriggerStay(Collider2D other)
        {
            if (currentInteractable == null) return;
            signObject.SetActive(currentInteractable.IsInteractable);
        }
    }

}
