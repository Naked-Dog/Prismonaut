using PlayerSystem;
using UnityEngine;

public class Charge : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBaseModule playerModule = collision.gameObject.GetComponentInParent<PlayerBaseModule>();
            playerModule.GetCharge();
            gameObject.SetActive(false);
        }
    }
}
