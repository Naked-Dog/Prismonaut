using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        if (MenuController.Instance)
        {
            MenuController.Instance.ChangeScene(sceneName);
            return;
        }

        SceneManager.LoadScene(sceneName);
        
    }
}
