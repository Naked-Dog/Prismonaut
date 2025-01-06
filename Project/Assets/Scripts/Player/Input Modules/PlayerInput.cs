using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public class PlayerInput
    {
        private InputActionAsset playerActions;
        private EventBus eventBus;
        private InputActionMap playerGameMap => playerActions.FindActionMap("Player");
        private InputActionMap playerUIMap => playerActions.FindActionMap("UI");
        private bool isPlayerInputPause => playerGameMap.enabled;

        public PlayerInput(EventBus eventBus, InputActionAsset playerInputAsset)
        {
            playerActions = playerInputAsset;
            InitializeActions();

            this.eventBus = eventBus;
            eventBus.Subscribe<UpdateEvent>(OnMove);
            eventBus.Subscribe<UpdateEvent>(OnJump);
            eventBus.Subscribe<PauseEvent>(OnPause);
            eventBus.Subscribe<UnpauseEvent>(OnUnpause);
        }

        private void OnUnpause(UnpauseEvent @event)
        {
            EnablePlayerInputs();
        }

        private void OnPause(PauseEvent @event)
        {
            DisablePlayerInputs();
        }

        private void InitializeActions()
        {
            playerGameMap.Enable();
            playerGameMap.Disable();

            playerGameMap.FindAction("TrianglePower").started += _ => eventBus.Publish(new TrianglePowerInputEvent());
            playerGameMap.FindAction("CirclePower").started += _ => eventBus.Publish(new CirclePowerInputEvent());
            playerGameMap.FindAction("SquarePower").started += _ => eventBus.Publish(new SquarePowerInputEvent(true));
            playerGameMap.FindAction("SquarePower").canceled += _ => eventBus.Publish(new SquarePowerInputEvent(false));

            playerGameMap.FindAction("Pause").started += _ =>
            {
                eventBus.Publish(new PauseEvent());
            };

            playerUIMap.FindAction("Pause").started += _ => 
            {
                eventBus.Publish(new UnpauseEvent());
            };
        }

        private void OnJump(UpdateEvent e)
        {
            eventBus.Publish(new JumpInputEvent(playerGameMap.FindAction("Jump")));
        }

        private void OnMove(UpdateEvent e)
        {
            float horizontalAxis = playerGameMap.FindAction("Move").ReadValue<float>();
            eventBus.Publish(new HorizontalInputEvent(horizontalAxis));
        }

        private void DisablePlayerInputs()
        {
            playerGameMap.Disable();
            playerUIMap.Enable();
        }

        private void EnablePlayerInputs()
        {
            playerGameMap.Enable();
            playerUIMap.Disable();
        }
    }
}

