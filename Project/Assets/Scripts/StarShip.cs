using Unity.AppUI.UI;
using UnityEngine;

public class StarShip : MonoBehaviour
{
    public void ChangeScene()
    {
        MenuController.Instance.ChangeScene("FinalScene");
    }
}
