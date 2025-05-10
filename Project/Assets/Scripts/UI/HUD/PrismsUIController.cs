using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismsUIController : MonoBehaviour
{
    //Referencias a vidas
    [SerializeField] private GameObject mainContainer;
    [SerializeField] private GameObject[] prisms;

    private readonly float PRISM_SHOW_TIME = 1f;

    public static PrismsUIController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InitUI(int currentPrism)
    {
        for (int i = 0; i < prisms.Length; i++)
        {
            prisms[i].SetActive(i + 1 <= currentPrism);
        }
    }

    public void UpdatePrismUI(int currentPrism)
    {
        for (int i = 0; i < prisms.Length; i++)
        {
            prisms[i].SetActive(i + 1 <= currentPrism);
        }
        if (!mainContainer.activeSelf) StartCoroutine(ShowHUDUI());
    }

    private IEnumerator ShowHUDUI()
    {
        mainContainer.SetActive(true);
        yield return new WaitForSeconds(PRISM_SHOW_TIME);
        mainContainer.SetActive(false);
    }
}
