using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUIController : MonoBehaviour
{
    //Referencias a vidas
    [SerializeField] private GameObject mainContainer;
    [SerializeField] private GameObject[] lives;

    private int maxHealth;
    private int currentHealth;

    private float HEALTH_SHOW_TIME = 1f;

    public void InitUI(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        foreach (GameObject live in lives)
        {
            live.SetActive(true);
        }
        mainContainer.SetActive(false);
    }

    //Actualizar vidas antes de aparecer
    public void UpdateHealthUI(int currentHealth)
    {
        this.currentHealth = currentHealth;
        for (int i = 0; i < lives.Length; i++)
        {
            lives[i].SetActive(i + 1 <= currentHealth);
        }
        StartCoroutine(ShowHealthUI());
    }

    public void ResetHealthUI()
    {
        currentHealth = maxHealth;
        foreach (GameObject live in lives)
        {
            live.SetActive(true);
        }
    }

    //Animacion de aparecer y desaparecer
    private IEnumerator ShowHealthUI()
    {
        mainContainer.SetActive(true);
        yield return new WaitForSeconds(HEALTH_SHOW_TIME);
        mainContainer.SetActive(false);
    }
}
