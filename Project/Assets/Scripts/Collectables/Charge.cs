using PlayerSystem;
using UnityEngine;

public class Charge : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("collected");
            GameObject player = collision.gameObject;
            Debug.Log(player);
            PlayerBaseModule playerModule = collision.gameObject.GetComponentInParent<PlayerBaseModule>();
            Debug.Log(playerModule == null);
            playerModule.GetCharge();
            gameObject.SetActive(false);
        }
    }
}
