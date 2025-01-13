using UnityEngine;

namespace PlayerSystem
{
    public class PlayerInteractionModule
    {
        private EventBus eventBus;
        private IInteractable currentInteractable;
        private GameObject signObject;

        public PlayerInteractionModule(EventBus eventBus, TriggerEventHandler triggerEvent, GameObject interactSign)
        {
            this.eventBus = eventBus;
            signObject = interactSign;
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
                eventBus.Publish(new PlayPlayerSounEffect("Interaction"));
                currentInteractable.Interact();
                signObject.SetActive(false);
            }
        }

        private void OnCollionEnter(Collider2D other)
        {
            var interactable = other.gameObject.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                signObject.SetActive(true);
            }
        }
        private void OnCollionExit(Collider2D other)
        {
            var interactable = other.gameObject.GetComponent<IInteractable>();

            if (currentInteractable == interactable)
            {
                currentInteractable = null;
                signObject.SetActive(false);
            }

        }

        private void OnTriggerStay(Collider2D other)
        {
            if (currentInteractable == null) return;
            //Debug.Log("gaaa");
            signObject.SetActive(currentInteractable.IsInteractable);
        }


    }

}
