using UnityEngine;

public class CoreManager : MonoBehaviour
{
    public static bool s_IsInitialized { get; private set; }

    private void Awake()
    {
        if (s_IsInitialized)
        {
            Destroy(gameObject);
            return;
        }

        s_IsInitialized = true;
        DontDestroyOnLoad(gameObject);
    }
}
