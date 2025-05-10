using PlayerSystem;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [SerializeField] private ShakeScriptable shakeProfile;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBaseModule playerModule = collision.gameObject.GetComponentInParent<PlayerBaseModule>();
            playerModule.GetCharge();
            GameManager.Instance.GetPrism();
            ShakeManager.Instance.CameraShake(shakeProfile);
            gameObject.SetActive(false);
        }
    }
}
