using System;
using System.Collections;
using UnityEngine;

public class ChargesUIController : MonoBehaviour
{
    [Serializable]
    private class Charge
    {
        public int maxCharges;
        public Sprite[] chargesSprites;
    }

    [SerializeField] private GameObject chargesContainer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Charge[] charges;

    private Charge setCharge;

    private float CHARGES_SHOW_TIME = 1f;

    public void InitChargesUI(int maxCharges)
    {
        SetChargesContainer(maxCharges);
        chargesContainer.SetActive(false);
    }

    //Actualizar vidas antes de aparecer
    public void UpdateChargesUI(int currentCharges)
    {
        if (currentCharges > setCharge.maxCharges) return;

        int chargeSpriteIndex = setCharge.maxCharges - currentCharges;
        sprite.sprite = setCharge.chargesSprites[chargeSpriteIndex];

        StartCoroutine(ShowChargesUI());
    }

    public void ResetChargesUI()
    {
        sprite.sprite = setCharge.chargesSprites[0];
    }

    public void SetChargesContainer(int maxCharges)
    {
        setCharge = Array.Find(charges, charge => charge.maxCharges == maxCharges);
        ResetChargesUI();
    }

    //Animacion de aparecer y desaparecer
    private IEnumerator ShowChargesUI()
    {
        chargesContainer.SetActive(true);
        yield return new WaitForSeconds(CHARGES_SHOW_TIME);
        chargesContainer.SetActive(false);
    }
}
