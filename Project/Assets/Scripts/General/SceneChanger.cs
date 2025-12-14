using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private SceneType sceneType;

    public void ChangeScene()
    {
        //Quick fix, removeeeeeeeeee later plssss
        AudioManager.Instance.Stop(EnvironmentSoundsEnum.Wind);
        SceneLoader.Instance.LoadScene(sceneType);
        return;
    }
}
