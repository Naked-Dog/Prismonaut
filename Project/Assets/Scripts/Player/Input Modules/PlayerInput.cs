using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public class PlayerInput
    {
        private InputActionAsset playerActions;
        private EventBus eventBus;
        private InputActionMap playerGameMap => playerActions.FindActionMap("Player") ;
        private InputActionMap playerUIMap => playerActions.FindActionMap("UI");

        public PlayerInput(EventBus eventBus, InputActionAsset playerInputAsset)
        {
            playerActions = playerInputAsset;
            InitializeActions();

            this.eventBus = eventBus;
            eventBus.Subscribe<UpdateEvent>(OnMove);
            eventBus.Subscribe<PauseEvent>(OnPause);
        }

        private void InitializeActions()
        {
            playerGameMap.Enable();
            playerUIMap.Disable();

            playerGameMap.FindAction("Jump").started += _ => eventBus.Publish(new JumpInputEvent());
            playerGameMap.FindAction("TrianglePower").started += _ => eventBus.Publish(new TrianglePowerInputEvent()); 
            playerGameMap.FindAction("CirclePower").started += _ => eventBus.Publish(new CirclePowerInputEvent()); 
            playerGameMap.FindAction("SquarePower").started += _ => eventBus.Publish(new SquarePowerInputEvent(true)); 
            playerGameMap.FindAction("SquarePower").canceled += _ => eventBus.Publish(new SquarePowerInputEvent(false));
            playerGameMap.FindAction("Pause").started += _ => OnPauseInput();

            playerUIMap.FindAction("Pause").started += _ => OnPauseInput(); 
        }

        private void OnMove(UpdateEvent e)
        {
            float horizontalAxis = playerGameMap.FindAction("Move").ReadValue<float>();
            eventBus.Publish(new HorizontalInputEvent(horizontalAxis));
        }

        private void OnPause(PauseEvent e)
        {
            if(playerGameMap.enabled)
            {
                playerGameMap.Disable();
                playerUIMap.Enable();
            } 
            else
            {
                playerGameMap.Enable();
                playerUIMap.Disable();
            }
        }

        private void OnPauseInput()
        {
            OnPause(new PauseEvent());
            eventBus.Publish(new PauseInputEvent());
        }

    }
}

