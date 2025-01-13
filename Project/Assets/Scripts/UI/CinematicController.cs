using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Video;

public class CinematicController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TextMeshProUGUI skipText;
    private InputDevice currentDevice;
    InputActionMap cinematicMap;
    private InputAction skipAction;
    private System.Action<InputAction.CallbackContext> skipActionDelegate;
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
        MenuController.Instance.ChangeScene("Level_1");
    }

    private void SkipCinematic()
    {
        videoPlayer.Stop();
        EnableInputs();
        skipAction.started -= skipActionDelegate;
        skipAction?.Disable();
        cinematicMap.Disable();
        MenuController.Instance.ChangeScene("Level_1");
    }

    private void Update()
    {
        var lastDevice = InputSystem.devices[InputSystem.devices.Count - 1];

        if (currentDevice != lastDevice)
        {
            currentDevice = lastDevice;
            TextChange(currentDevice);
        }
    }

    private void TextChange(InputDevice device)
    {
        Debug.Log(device);
        if(device is Keyboard)
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
