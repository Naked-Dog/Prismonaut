using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnsureCoreInEditor : MonoBehaviour
{
    [SerializeField] private string m_coreSceneName = "Core";

    private void Awake()
    {
#if UNITY_EDITOR
        StartCoroutine(EnsureCoreLoaded());
#endif
    }

    private IEnumerator EnsureCoreLoaded()
    {
        if (CoreManager.s_IsInitialized)
        {
            Debug.Log("@EnsureCoreInEditor - Core initialized.");
            yield break;
        }

        yield return SceneManager.LoadSceneAsync(m_coreSceneName, LoadSceneMode.Additive);
        yield return null;
        Debug.Log($"@EnsureCoreInEditor - Core loaded to test: {SceneManager.GetActiveScene().name}");
    }
}
