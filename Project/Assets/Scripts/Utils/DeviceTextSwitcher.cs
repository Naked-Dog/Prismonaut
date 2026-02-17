using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DeviceTextSwitcher : MonoBehaviour
{
    [Header("Texts Per Device")]
    [SerializeField] private string keyboardText;
    [SerializeField] private string gamepadText;
    [SerializeField] private string touchText;

    [Header("Optional Action Reference")]
    [SerializeField] private InputActionReference listenAction;

    private TextMeshProUGUI textUI;

    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (listenAction != null)
        {
            listenAction.action.performed += OnActionPerformed;
            listenAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (listenAction != null)
        {
            listenAction.action.performed -= OnActionPerformed;
        }
    }

    private void OnActionPerformed(InputAction.CallbackContext ctx)
    {
        var device = ctx.control.device;
        UpdateText(device);
    }

    private void UpdateText(InputDevice device)
    {
        if (Application.isMobilePlatform)
        {
            textUI.text = touchText;
            return;
        }

        if (device is Gamepad)
        {
            textUI.text = gamepadText;
        }
        else
        {
            textUI.text = keyboardText;
        }
    }
}
