using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SecretEntrance : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;
    [SerializeField] private Tilemap fakeTileMap;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FadeAndGone());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FadeAndGone());
        }
    }

    IEnumerator FadeAndGone()
    {
        parentObject.GetComponent<Collider2D>().enabled = false;

        AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Heal);

        if (fakeTileMap != null)
        {
            float fadeSpeed = 1f;

            while (fakeTileMap.color.a > 0)
            {
                float newAlpha = fakeTileMap.color.a - fadeSpeed * Time.deltaTime;

                Color newColor = new Color(fakeTileMap.color.r, fakeTileMap.color.g, fakeTileMap.color.b, newAlpha);
                fakeTileMap.color = newColor;

                yield return null;
            }

            Color finalColor = fakeTileMap.color;
            finalColor.a = 0;
            fakeTileMap.color = finalColor;
            Destroy(parentObject);
        }
    }

}
