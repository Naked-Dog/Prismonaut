using PlayerSystem;
using UnityEngine;

public class Prism : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBaseModule playerModule = collision.gameObject.GetComponentInParent<PlayerBaseModule>();
            playerModule.GetCharge();
            GameManager.Instance.GetPrism();
            gameObject.SetActive(false);
        }
    }
}
