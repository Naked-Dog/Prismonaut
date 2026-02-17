using UnityEngine;

public class UIControllerMenu : UIControllerBase
{
    [Header("References")]
    [SerializeField] private PanelBase m_settingsPanel;
    [SerializeField] private PanelBase m_creditsPanel;

    public void OnPlayButton()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(SceneType.StartCinematic);
        }
        else
        {
            Debug.LogError("[UIControllerMenu] SceneLoader.Instance is null on play.");
        }
    }

    public void OnSettingsButton()
    {
        if (m_settingsPanel != null)
        {
            OpenPanel(m_settingsPanel);
        }
        else
        {
            Debug.LogWarning("[UIControllerMenu] Settings panel is null on settings button.");
        }
    }

    public void OnCreditsButton()
    {
        if (m_creditsPanel != null)
        {
            OpenPanel(m_creditsPanel);
        }
        else
        {
            Debug.LogWarning("[UIControllerMenu] Credits panel is null on credits button.");
        }
    }

    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
