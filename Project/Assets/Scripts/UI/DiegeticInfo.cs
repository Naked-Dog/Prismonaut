using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class DiegeticInfo : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public Image backgroundImage;

    public static DiegeticInfo Instance { get; private set; }

    private Coroutine currentRoutine;
    private InputDevice currentDevice;

    private string keyboardText;
    private string controllerText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (backgroundImage != null)
        {
            var color = backgroundImage.color;
            backgroundImage.color = new Color(color.r, color.g, color.b, 0);
        }

        if (textMesh != null)
        {
            textMesh.alpha = 0;
        }
    }

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

    public void ShowDiegeticInfo(DiegeticInfoType diegeticInfoType)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(SetDiegeticInfo(diegeticInfoType));
    }

    public IEnumerator SetDiegeticInfo(DiegeticInfoType diegeticInfoType)
    {
        this.keyboardText = diegeticInfoType.keyboardText;
        if(diegeticInfoType.controllerText != null)this.controllerText = diegeticInfoType.controllerText;

        if (backgroundImage == null || textMesh == null)
        {
            Debug.LogError("Missing Image or TextMeshProUGUI component on DiegeticInfo.");
            yield break;
        }

        backgroundImage.gameObject.SetActive(false);
        textMesh.alpha = 0;
        backgroundImage.gameObject.SetActive(true);

        TextChange(currentDevice);

        Sequence fadeIn = DOTween.Sequence();
        fadeIn.Join(backgroundImage.DOFade(0.75f, 0.5f));
        fadeIn.Join(textMesh.DOFade(1, 0.5f));
        yield return fadeIn.WaitForCompletion();

        yield return new WaitForSeconds(diegeticInfoType.time);

        Sequence fadeOut = DOTween.Sequence();
        fadeOut.Join(backgroundImage.DOFade(0, 0.5f));
        fadeOut.Join(textMesh.DOFade(0, 0.5f));
        yield return fadeOut.WaitForCompletion();

        backgroundImage.gameObject.SetActive(false);
    }

    private void TextChange(InputDevice device)
    {
        if (device is Keyboard || device is Mouse)
        {
            textMesh.text = keyboardText;
        }
        else
        {
            textMesh.text = controllerText;
        }
    }
}

[Serializable]
public class DiegeticInfoType
{
    public float time = 2f;
    public string keyboardText;
    public string controllerText = null;
}
