using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        if (MenuController.Instance)
        {
            //Quick fix, removeeeeeeeeee later plssss
            AudioManager.Instance.Stop(BullSoundsEnum.BossFinishZone);
            MenuController.Instance.ChangeScene(sceneName);
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
