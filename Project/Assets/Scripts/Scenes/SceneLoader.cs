using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private SceneDatabase m_sceneDatabase;
    [SerializeField] private float m_fadeDuration = 0.5f;

    private string m_currentScene;

    public string CurrentScene => m_currentScene;

    public event Action<string> OnSceneLoaded;

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
        m_currentScene = SceneManager.GetActiveScene().name;
    }

    public void LoadScene(SceneType sceneType)
    {
        if (m_sceneDatabase == null)
        {
            Debug.LogError("@SceneLoader - SceneDatabase not assigned to SceneLoader!");
            return;
        }

        var entry = m_sceneDatabase.GetEntry(sceneType);
        if (entry == null)
        {
            Debug.LogError($"@SceneLoader - Scene entry not found for {sceneType}");
            return;
        }

        StartCoroutine(LoadSceneRoutine(entry));
    }

    private IEnumerator LoadSceneRoutine(SceneDatabase.SceneEntry entry)
    {
        if (UIManager.Instance != null)
            yield return UIManager.Instance.FadeToBlack(m_fadeDuration);

        yield return SceneManager.LoadSceneAsync(entry.SceneName, LoadSceneMode.Single);

        m_currentScene = entry.SceneName;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(entry.SceneName));

        if (entry != null && AudioManager.Instance != null && !AudioManager.Instance.IsSameMusicPlaying(entry.MusicKey))
            AudioManager.Instance.PlayMusic(entry.MusicKey);

        if (UIManager.Instance != null)
            yield return UIManager.Instance.FadeFromBlack(m_fadeDuration);

        OnSceneLoaded?.Invoke(m_currentScene);
    }

    public void PlayMusicForScene(string sceneName)
    {
        if (m_sceneDatabase == null) return;

        var entry = m_sceneDatabase.Scenes.Find(s => s.SceneName == sceneName);
        if (entry != null && AudioManager.Instance != null && !AudioManager.Instance.IsSameMusicPlaying(entry.MusicKey))
        {
            AudioManager.Instance.PlayMusic(entry.MusicKey);
        }
    }
}
