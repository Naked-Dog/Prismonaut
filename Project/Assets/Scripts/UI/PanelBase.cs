using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PanelBase : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] private float m_fadeDuration = 0.2f;
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private GameObject m_firstSelected;

    [Header("Events")]
    public UnityEvent OnOpen;
    public UnityEvent OnClose;

    public bool isOpen { get; private set; }

    private void Awake()
    {
        if (m_canvasGroup == null)
            m_canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Open()
    {
        if (isOpen) return;
        isOpen = true;

        gameObject.SetActive(true);
        m_canvasGroup.DOKill();
        m_canvasGroup.alpha = 0;
        m_canvasGroup.interactable = true;
        m_canvasGroup.blocksRaycasts = true;

        m_canvasGroup.DOFade(1, m_fadeDuration).SetUpdate(true);

        if (m_firstSelected != null)
            EventSystem.current.SetSelectedGameObject(m_firstSelected);

        OnOpen?.Invoke();
    }

    public virtual void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        OnClose?.Invoke();
        m_canvasGroup.DOKill();
        m_canvasGroup.interactable = false;
        m_canvasGroup.blocksRaycasts = false;

        m_canvasGroup.DOFade(0, m_fadeDuration)
            .SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void HideInstant()
    {
        isOpen = false;
        m_canvasGroup.DOKill();
        m_canvasGroup.alpha = 0;
        m_canvasGroup.interactable = false;
        m_canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    public void ShowInstant()
    {
        isOpen = true;
        m_canvasGroup.DOKill();
        m_canvasGroup.alpha = 1;
        m_canvasGroup.interactable = true;
        m_canvasGroup.blocksRaycasts = true;
        gameObject.SetActive(true);
    }
}
