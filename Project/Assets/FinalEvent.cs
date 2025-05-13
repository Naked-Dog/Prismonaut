using UnityEngine;

public class FinalEvent : MonoBehaviour
{
    public void GoToMenu()
    {
        MenuController.Instance?.ChangeScene("Menu");
    }
}
