using UnityEngine;

namespace PlayerSystem
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        private EventBus eventBus;

        public void SetEventBus(EventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public void PublishRespawnEvent(){
            eventBus.Publish(new RespawnEvent());
        }

        public void PlaySound(string clipName)
        {
            eventBus.Publish(new PlayPlayerSounEffect(clipName));
        }
    }
}
