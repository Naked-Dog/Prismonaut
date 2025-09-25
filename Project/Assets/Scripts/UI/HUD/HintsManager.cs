using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum HintType
{
    Dash,
    Drill,
    Parry,
    Charges2,
    Charges3,
    Boss,
}

[Serializable]
public class HintData
{
    public HintType type;
    public Sprite sprite;
}

public class HintsManager : MonoBehaviour
{
    public static HintsManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image hintSpriteImage;
    [SerializeField] private Image backgroundImage;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayTime = 2.5f;
    [SerializeField] private Ease fadeInEase = Ease.OutQuad;
    [SerializeField] private Ease fadeOutEase = Ease.InQuad;

    [Header("Hints")]
    [SerializeField] private List<HintData> hints = new List<HintData>();

    private Dictionary<HintType, Sprite> hintDictionary;
    private Tween currentTween;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        hintDictionary = new Dictionary<HintType, Sprite>();
        foreach (var hint in hints)
        {
            if (!hintDictionary.ContainsKey(hint.type))
                hintDictionary.Add(hint.type, hint.sprite);
        }
        panel.SetActive(false);
    }

    public void ShowHintByIndex(int typeIndex)
    {
        if (!Enum.IsDefined(typeof(HintType), typeIndex))
            return;

        ShowHint((HintType)typeIndex);
    }

    public void ShowHint(HintType type)
    {
        if (!hintDictionary.ContainsKey(type) || hintSpriteImage == null || backgroundImage == null || panel == null)
            return;

        currentTween?.Kill();

        hintSpriteImage.sprite = hintDictionary[type];

        backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 0);
        hintSpriteImage.color = new Color(hintSpriteImage.color.r, hintSpriteImage.color.g, hintSpriteImage.color.b, 0);

        panel.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(backgroundImage.DOFade(0.75f, fadeDuration).SetEase(fadeInEase));
        seq.Join(hintSpriteImage.DOFade(1f, fadeDuration).SetEase(fadeInEase));
        seq.AppendInterval(displayTime);
        seq.Append(backgroundImage.DOFade(0f, fadeDuration).SetEase(fadeOutEase));
        seq.Join(hintSpriteImage.DOFade(0f, fadeDuration).SetEase(fadeOutEase));
        seq.OnComplete(() =>
        {
            if (panel != null)
                panel.SetActive(false);
        });

        currentTween = seq;
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
    }
}
