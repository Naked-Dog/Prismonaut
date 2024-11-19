using UnityEngine;

public class StarShip : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            if(MenuController.Instance){
                MenuController.Instance.ChangeScene(sceneName);
            }
            Debug.Log("Cambio de Scena");
        }
    }
}
