using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [Header("Bootstrap Settings")]
    [SerializeField] private SceneType m_firstScene = SceneType.MainMenu;

    private void Start()
    {
        if (SceneManager.sceneCount > 1) return;

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(m_firstScene);
        }
        else
        {
            Debug.LogError("@Bootstrapper - SceneLoader not found in Core!");
        }
    }
}
