using UnityEngine;

public class UIControllerMenu : UIControllerBase
{
    [Header("References")]
    [SerializeField] private PanelBase m_settingsPanel;
    [SerializeField] private PanelBase m_creditsPanel;

    public void OnPlayButton() =>
        SceneLoader.Instance.LoadScene(SceneType.StartCinematic);

    public void OnSettingsButton() => OpenPanel(m_settingsPanel);
    public void OnCreditsButton() => OpenPanel(m_creditsPanel);

    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
