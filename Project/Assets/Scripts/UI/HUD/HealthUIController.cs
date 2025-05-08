using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIController : MonoBehaviour
{
    //Referencias a vidas
    [SerializeField] private GameObject mainContainer;
    [SerializeField] private GameObject[] lives;

    [SerializeField] private Image portraitContainer;

    [SerializeField] private Sprite[] portraits;

    private enum Portraits
    {
        Full,
        Damaged,
        Dead
    }

    private int maxHealth;

    private float HEALTH_SHOW_TIME = 1f;

    public void InitUI(int maxHealth)
    {
        this.maxHealth = maxHealth;
        foreach (GameObject live in lives)
        {
            live.SetActive(true);
        }
        mainContainer.SetActive(false);
    }

    //Actualizar vidas antes de aparecer
    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < lives.Length; i++)
        {
            lives[i].SetActive(i + 1 <= currentHealth);
        }
        SetPortraitImage(currentHealth);

        if (currentHealth == maxHealth)
        {
            StartCoroutine(ShowHUDUI());
        }
        else
        {
            mainContainer.SetActive(true);
        }
    }

    private void SetPortraitImage(int currentHealth)
    {
        if (currentHealth == 0)
        {
            portraitContainer.sprite = portraits[(int)Portraits.Dead];
        }
        else if (currentHealth == maxHealth)
        {
            portraitContainer.sprite = portraits[(int)Portraits.Full];
        }
        else
        {
            portraitContainer.sprite = portraits[(int)Portraits.Damaged];
        }
    }

    public void ResetHealthUI()
    {
        portraitContainer.sprite = portraits[(int)Portraits.Full];
        foreach (GameObject live in lives)
        {
            live.SetActive(true);
        }
    }

    //Animacion de aparecer y desaparecer
    private IEnumerator ShowHUDUI()
    {
        mainContainer.SetActive(true);
        yield return new WaitForSeconds(HEALTH_SHOW_TIME);
        mainContainer.SetActive(false);
    }
}
