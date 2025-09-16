using System;
using System.Collections.Generic;
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
        private InputActionMap DialogueMap => playerActions.FindActionMap("Dialogue");

        private List<(InputAction action, Action<InputAction.CallbackContext> callback)> registeredCallbacks =
            new List<(InputAction, Action<InputAction.CallbackContext>)>();

        public PlayerInput(EventBus eventBus, InputActionAsset playerInputAsset)
        {
            playerActions = playerInputAsset;
            InitializeActions();

            this.eventBus = eventBus;
            eventBus.Subscribe<OnUpdate>(PublishInputEvents);
            eventBus.Subscribe<RequestPlayerInputs>(EnablePlayerMapInput);
            eventBus.Subscribe<RequestStopPlayerInputs>(StopPlayerMapInput);
            eventBus.Subscribe<RequestEnableDialogueInputs>(EnableDialogueInputs);
            eventBus.Subscribe<RequestDisableDialogueInputs>(DiableDialogueInputs);
        }

        private void StopPlayerMapInput(RequestStopPlayerInputs e)
        {
            playerGameMap.Disable();
        }

        private void EnablePlayerMapInput(RequestPlayerInputs e)
        {
            playerGameMap.Enable();
        }

        private void EnableDialogueInputs(RequestEnableDialogueInputs @event)
        {
            DialogueMap.Enable();
            playerGameMap.Disable();
        }

        private void DiableDialogueInputs(RequestDisableDialogueInputs @event)
        {
            DialogueMap.Disable();
            playerGameMap.Enable();
        }

        private void InitializeActions()
        {
            playerUIMap.Disable();
            DialogueMap.Disable();

            RegisterCallback(playerGameMap.FindAction("Jump"), ctx => eventBus.Publish(new OnJumpInput(ctx)));
            RegisterCallback(playerGameMap.FindAction("TrianglePower"), ctx => eventBus.Publish(new OnTrianglePowerInput()));
            RegisterCallback(playerGameMap.FindAction("CirclePower"), ctx => eventBus.Publish(new OnCirclePowerInput()));
            RegisterCallback(playerGameMap.FindAction("SquarePower"), ctx => eventBus.Publish(new OnSquarePowerInput(ctx)));

            RegisterCallback(playerGameMap.FindAction("Interaction"), ctx => eventBus.Publish(new OnInteractionInput()));

            playerGameMap.FindAction("LookDown").started += LookDownInput;
            playerGameMap.FindAction("LookDown").canceled += LookDownInput;

            playerGameMap.FindAction("LookUp").started += LookUpInput;
            playerGameMap.FindAction("LookUp").canceled += LookUpInput;


            RegisterCallback(playerGameMap.FindAction("Pause"), ctx =>
            {
                eventBus.Publish(new OnPauseInput());
                eventBus.Publish(new RequestPause());
            });

            RegisterCallback(playerUIMap.FindAction("Pause"), ctx =>
            {
                eventBus.Publish(new OnPauseInput());
                eventBus.Publish(new RequestUnpause());
            });

            RegisterCallback(DialogueMap.FindAction("SkipDialogue"), ctx => DialogueController.Instance?.SkipDialogue());
        }

        private void RegisterCallback(InputAction action, Action<InputAction.CallbackContext> callback)
        {
            action.started += callback;
            action.performed += callback;
            action.canceled += callback;

            registeredCallbacks.Add((action, callback));
        }

        private void PublishInputEvents(OnUpdate e)
        {
            float horizontalAxis = playerGameMap.FindAction("Move").ReadValue<float>();
            eventBus.Publish(new OnHorizontalInput(horizontalAxis));

            float verticalAxis = playerGameMap.FindAction("Vertical").ReadValue<float>();
            eventBus.Publish(new OnVerticalInput(verticalAxis));
        }

        private void DisablePlayerInputs()
        {
            Debug.Log("Disable");
            playerGameMap.Disable();
        }

        private void EnablePlayerInputs()
        {
            Debug.Log("Enable");
            playerGameMap.Enable();
        }

        private void LookDownInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                eventBus.Publish(new OnLookDownInput(true));
            }
            else if (ctx.canceled)
            {
                eventBus.Publish(new OnLookDownInput(false));
            }
        }

        private void LookUpInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                eventBus.Publish(new OnLookUpInput(true));
            }
            else if (ctx.canceled)
            {
                eventBus.Publish(new OnLookUpInput(false));
            }
        }

        public void Dispose()
        {
            eventBus.Unsubscribe<OnUpdate>(PublishInputEvents);
            eventBus.Unsubscribe<RequestPlayerInputs>(EnablePlayerMapInput);
            eventBus.Unsubscribe<RequestStopPlayerInputs>(StopPlayerMapInput);
            eventBus.Unsubscribe<RequestEnableDialogueInputs>(EnableDialogueInputs);
            eventBus.Unsubscribe<RequestDisableDialogueInputs>(DiableDialogueInputs);

            playerGameMap.FindAction("LookDown").started -= LookDownInput;
            playerGameMap.FindAction("LookDown").canceled -= LookDownInput;
            
            playerGameMap.FindAction("LookUp").started -= LookUpInput;
            playerGameMap.FindAction("LookUp").canceled -= LookUpInput;

            foreach (var (action, callback) in registeredCallbacks)
            {
                action.started -= callback;
                action.performed -= callback;
                action.canceled -= callback;
            }

            registeredCallbacks.Clear();

            playerActions = null;
            eventBus = null;
        }
    }
}

