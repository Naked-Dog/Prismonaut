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
            AudioManager.Instance.Play2DSound(LevelEventsSoundsEnum.Prism);
            gameObject.SetActive(false);
        }
    }
}
