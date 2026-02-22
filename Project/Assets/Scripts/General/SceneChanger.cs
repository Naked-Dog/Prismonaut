using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private SceneType sceneType;

    public void ChangeScene(bool willFade = true)
    {
        //Quick fix, removeeeeeeeeee later plssss
        AudioManager.Instance.Stop(EnvironmentSoundsEnum.Wind);
        SceneLoader.Instance.LoadScene(sceneType, willFade);
        return;
    }
}
