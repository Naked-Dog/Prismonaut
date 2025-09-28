using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Video;

public class CinematicController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TextMeshProUGUI skipText;
    private InputDevice currentDevice;
    InputActionMap cinematicMap;
    private InputAction skipAction;
    private System.Action<InputAction.CallbackContext> skipActionDelegate;

    private void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        if (device != currentDevice)
        {
            currentDevice = device;
            TextChange(device);
        }
    }

    private void Start()
    {
        StartCoroutine(MenuController.Instance.FadeOutSolidPanel());
        videoPlayer.loopPointReached += OnVideoEnd;
        DisableInputs();
        cinematicMap = InputSystem.actions.FindActionMap("Cinematic");
        skipAction = cinematicMap.FindAction("Skip");

        skipActionDelegate = context => SkipCinematic();
        skipAction.started += skipActionDelegate;

        skipAction.Enable();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        MenuController.Instance.ChangeScene("Beta_Level_1");
    }

    private void SkipCinematic()
    {
        AudioManager.Instance.Play2DSound(DialogueSoundsEnum.Skip);
        videoPlayer.Stop();
        EnableInputs();
        skipAction.started -= skipActionDelegate;
        skipAction?.Disable();
        cinematicMap.Disable();
        MenuController.Instance.ChangeScene("Beta_Level_1");
    }

    private void TextChange(InputDevice device)
    {
        if (device is Keyboard || device is Mouse)
        {
            skipText.text = "Esc to skip";
        }
        else
        {
            skipText.text = "Start to skip";
        }
    }

    private void DisableInputs()
    {
        InputSystem.actions.FindActionMap("Player").Disable();
        InputSystem.actions.FindActionMap("UI").Disable();
        InputSystem.actions.FindActionMap("Dialogue").Disable();
    }

    private void EnableInputs()
    {
        InputSystem.actions.FindActionMap("Player").Enable();
        InputSystem.actions.FindActionMap("UI").Enable();
        InputSystem.actions.FindActionMap("Dialogue").Enable();
    }
}
