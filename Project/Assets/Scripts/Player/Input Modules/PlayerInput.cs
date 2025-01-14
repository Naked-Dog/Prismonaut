using System;
using System.Collections.Generic;
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
            eventBus.Subscribe<UpdateEvent>(OnMove);
            eventBus.Subscribe<UpdateEvent>(OnJump);
            eventBus.Subscribe<EnablePlayerInputsEvent>(EnablePlayerMapInput);
            eventBus.Subscribe<StopPlayerInputsEvent>(StopPlayerMapInput);
            eventBus.Subscribe<EnableDialogueInputsEvent>(EnableDialogueInputs);
            eventBus.Subscribe<DisableDilagueInputsEvent>(DiableDialogueInputs);
        }

        private void StopPlayerMapInput(StopPlayerInputsEvent e)
        {
            DisablePlayerInputs();
        }

        private void EnablePlayerMapInput(EnablePlayerInputsEvent e)
        {
            EnablePlayerInputs();
        }

        private void EnableDialogueInputs(EnableDialogueInputsEvent @event)
        {
            DialogueMap.Enable();
            playerGameMap.Disable();
        }

        private void DiableDialogueInputs(DisableDilagueInputsEvent @event)
        {
            DialogueMap.Disable();
            playerGameMap.Enable();
        }

        private void InitializeActions()
        {
            playerUIMap.Disable();
            DialogueMap.Disable();

            RegisterCallback(playerGameMap.FindAction("TrianglePower"), ctx => eventBus.Publish(new TrianglePowerInputEvent()));
            RegisterCallback(playerGameMap.FindAction("CirclePower"), ctx => eventBus.Publish(new CirclePowerInputEvent()));
            playerGameMap.FindAction("SquarePower").started += SquarePowerInput;
            playerGameMap.FindAction("SquarePower").canceled += SquarePowerInput;

            RegisterCallback(playerGameMap.FindAction("Interaction"), ctx => eventBus.Publish(new InteractionInputEvent()));

            playerGameMap.FindAction("LookDown").started += LookDownInput;
            playerGameMap.FindAction("LookDown").canceled += LookDownInput;


            RegisterCallback(playerGameMap.FindAction("Pause"), ctx =>
            {
                eventBus.Publish(new PauseInputEvent());
                eventBus.Publish(new PauseEvent());
            });

            RegisterCallback(playerUIMap.FindAction("Pause"), ctx =>
            {
                eventBus.Publish(new PauseInputEvent());
                eventBus.Publish(new UnpauseEvent());
            });

            RegisterCallback(DialogueMap.FindAction("SkipDialogue"), ctx => DialogueController.Instance.SkipDialogue());
        }

        private void RegisterCallback(InputAction action, Action<InputAction.CallbackContext> callback, bool started = true, bool canceled = false)
        {
            if (started) action.started += callback;
            if (canceled) action.canceled += callback;

            registeredCallbacks.Add((action, callback));
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

        private void SquarePowerInput(InputAction.CallbackContext ctx)
        {
            if(ctx.started)
            {
                eventBus.Publish(new SquarePowerInputEvent(true));
            } 
            else if (ctx.canceled)
            {
                eventBus.Publish(new SquarePowerInputEvent(false));
            }
        }

        private void LookDownInput(InputAction.CallbackContext ctx)
        {
            if(ctx.started)
            {
                eventBus.Publish(new LookDownInputEvent(true));
            } 
            else if (ctx.canceled)
            {
                eventBus.Publish(new LookDownInputEvent(false));
            }
        }

        public void Dispose()
        {
            eventBus.Unsubscribe<UpdateEvent>(OnMove);
            eventBus.Unsubscribe<UpdateEvent>(OnJump);
            eventBus.Unsubscribe<EnablePlayerInputsEvent>(EnablePlayerMapInput);
            eventBus.Unsubscribe<StopPlayerInputsEvent>(StopPlayerMapInput);
            eventBus.Unsubscribe<EnableDialogueInputsEvent>(EnableDialogueInputs);
            eventBus.Unsubscribe<DisableDilagueInputsEvent>(DiableDialogueInputs);

            playerGameMap.FindAction("SquarePower").started -= SquarePowerInput;
            playerGameMap.FindAction("SquarePower").canceled -= SquarePowerInput;
            playerGameMap.FindAction("LookDown").started -= LookDownInput;
            playerGameMap.FindAction("LookDown").canceled -= LookDownInput;

            foreach (var (action, callback) in registeredCallbacks)
            {
                action.started -= callback;
                action.canceled -= callback;
            }

            registeredCallbacks.Clear();

            playerActions = null;
            eventBus = null;
        }
    }
}

