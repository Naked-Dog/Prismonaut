using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Image fillRenderer;
    [SerializeField] private float time;
    
    private float timeLeft;

    void Start() {
        timeLeft = time;
        StartCoroutine(myCoroutine());
    }

    IEnumerator myCoroutine() {
        while (timeLeft > 0) {
            timeLeft--;
            fillRenderer.fillAmount = timeLeft / time;
            yield return new WaitForSeconds(1f);
        }
    }
}