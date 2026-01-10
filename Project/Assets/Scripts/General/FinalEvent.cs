using UnityEngine;

public class FinalEvent : MonoBehaviour
{
    public void GoToMenu()
    {
        SceneLoader.Instance.LoadScene(SceneType.MainMenu);
    }
}
