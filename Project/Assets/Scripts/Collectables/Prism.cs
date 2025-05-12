using PlayerSystem;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [SerializeField] private ShakeScriptable shakeProfile;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBaseModule.Instance.GetPrism();
            ShakeManager.Instance.CameraShake(shakeProfile);
            gameObject.SetActive(false);
        }
    }
}
