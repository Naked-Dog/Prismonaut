using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismsUIController : MonoBehaviour
{
    //Referencias a vidas
    [SerializeField] private GameObject mainContainer;
    [SerializeField] private GameObject[] prisms;

    private float PRISM_SHOW_TIME = 1f;

    public void InitUI(int currentPrism)
    {
        for (int i = 0; i < prisms.Length; i++)
        {
            prisms[i].SetActive(i + 1 <= currentPrism);
        }
        mainContainer.SetActive(false);
    }

    //Actualizar vidas antes de aparecer
    public void UpdatePrismUI(int currentPrism)
    {
        for (int i = 0; i < prisms.Length; i++)
        {
            prisms[i].SetActive(i + 1 <= currentPrism);
        }
        StartCoroutine(ShowHUDUI());
    }

    //Animacion de aparecer y desaparecer
    private IEnumerator ShowHUDUI()
    {
        mainContainer.SetActive(true);
        yield return new WaitForSeconds(PRISM_SHOW_TIME);
        mainContainer.SetActive(false);
    }
}
