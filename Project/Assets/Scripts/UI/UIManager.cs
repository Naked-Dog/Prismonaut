using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private CanvasGroup m_fadeCanvas;
    [SerializeField] private float m_defaultFadeDuration = 0.5f;

    private Coroutine m_currentFadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_fadeCanvas.alpha = 1f;
        StartCoroutine(FadeFromBlack());
    }

    public IEnumerator FadeToBlack(float duration = -1f)
    {
        yield return FadeRoutine(1, duration < 0 ? m_defaultFadeDuration : duration);
    }

    public IEnumerator FadeFromBlack(float duration = -1f)
    {
        yield return FadeRoutine(0, duration < 0 ? m_defaultFadeDuration : duration);
    }

    public void InstantBlack()
    {
        SetAlpha(1);
    }

    public void InstantClear()
    {
        SetAlpha(0);
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        yield return FadeCoroutine(targetAlpha, duration);
    }

    private IEnumerator FadeCoroutine(float targetAlpha, float duration)
    {
        m_fadeCanvas.blocksRaycasts = true;

        float startAlpha = m_fadeCanvas.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            m_fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        m_fadeCanvas.alpha = targetAlpha;

        m_fadeCanvas.blocksRaycasts = targetAlpha > 0.9f;
    }

    private void SetAlpha(float value)
    {
        m_fadeCanvas.alpha = value;
        m_fadeCanvas.blocksRaycasts = value > 0.9f;
    }
}
