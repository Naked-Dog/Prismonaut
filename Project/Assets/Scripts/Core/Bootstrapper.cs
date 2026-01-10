using UnityEngine;
using System.Collections;

public class Bootstrapper : MonoBehaviour
{
    [Header("Bootstrap Settings")]
    [SerializeField] private SceneType m_firstScene = SceneType.MainMenu;
    [SerializeField] private bool m_forceFullFlowInEditor = false;

    private IEnumerator Start()
    {
#if UNITY_EDITOR
        if (!m_forceFullFlowInEditor)
        {
            GetComponent<AudioListener>().enabled = false;
            yield break;
        }
#endif

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
