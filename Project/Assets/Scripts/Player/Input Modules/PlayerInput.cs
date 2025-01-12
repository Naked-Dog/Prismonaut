using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace PlayerSystem
{
    public class PlayerInput
    {
        private InputActionAsset playerActions;
        private EventBus eventBus;
        private InputActionMap playerGameMap => playerActions.FindActionMap("Player");
        private InputActionMap playerUIMap => playerActions.FindActionMap("UI");

        public PlayerInput(EventBus eventBus, InputActionAsset playerInputAsset)
        {
            playerActions = playerInputAsset;
            InitializeActions();

            this.eventBus = eventBus;
            eventBus.Subscribe<UpdateEvent>(OnMove);
            eventBus.Subscribe<UpdateEvent>(OnJump);
            eventBus.Subscribe<EnablePlayerInputsEvent>(EnablePlayerMapInput);
            eventBus.Subscribe<StopPlayerInputsEvent>(StopPlayerMapInput);
        }

        private void StopPlayerMapInput(StopPlayerInputsEvent e)
        {
            DisablePlayerInputs();
        }

        private void EnablePlayerMapInput(EnablePlayerInputsEvent e)
        {
            EnablePlayerInputs();
        }

        private void InitializeActions()
        {
            playerUIMap.Disable();

            playerGameMap.FindAction("TrianglePower").started += _ => eventBus.Publish(new TrianglePowerInputEvent());
            playerGameMap.FindAction("CirclePower").started += _ => eventBus.Publish(new CirclePowerInputEvent());
            playerGameMap.FindAction("SquarePower").started += _ => eventBus.Publish(new SquarePowerInputEvent(true));
            playerGameMap.FindAction("SquarePower").canceled += _ => eventBus.Publish(new SquarePowerInputEvent(false));
            playerGameMap.FindAction("Interaction").started += _ => eventBus.Publish(new InteractionInputEvent());
            playerGameMap.FindAction("LookDown").started += _ => eventBus.Publish(new LookDownInputEvent(true));
            playerGameMap.FindAction("LookDown").canceled += _ => eventBus.Publish(new LookDownInputEvent(false));


            playerGameMap.FindAction("Pause").started += _ =>
            {
                eventBus.Publish(new PauseInputEvent());
                eventBus.Publish(new PauseEvent());
            };

            playerUIMap.FindAction("Pause").started += _ => 
            {
                eventBus.Publish(new PauseInputEvent());
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

