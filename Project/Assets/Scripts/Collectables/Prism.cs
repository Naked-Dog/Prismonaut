using PlayerSystem;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [SerializeField] private ShakeScriptable shakeProfile;
    [SerializeField] private GameObject prismSprite;
    [SerializeField] private GameObject shine;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBaseModule.Instance.GetPrism();
            ShakeManager.Instance.CameraShake(shakeProfile);
            AudioManager.Instance.Play2DSound(LevelEventsSoundsEnum.Prism);
            GetComponent<Collider2D>().enabled = false;
            prismSprite.SetActive(false);
            shine.SetActive(true);
            ShineCanvas.OnShineBackground?.Invoke();
        }
    }
}
