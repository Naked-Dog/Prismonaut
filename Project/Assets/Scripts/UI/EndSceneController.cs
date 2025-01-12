using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class EndSceneController : MonoBehaviour
{

    [SerializeField] private Image logo;
    [SerializeField] private Image message;

    void Start()
    {
        StartCoroutine(StartScene());
    }

    private IEnumerator FadeInImage(Image image)
    {
        image.gameObject.SetActive(true);
        Tween fadeIn = image.DOFade(1, 3f);
        yield return fadeIn.WaitForCompletion();
    }

    private IEnumerator StartScene()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeInImage(logo));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeInImage(message));
        yield return new WaitForSeconds(6f);
        MenuController.Instance.ChangeScene("Menu");
    }
}
