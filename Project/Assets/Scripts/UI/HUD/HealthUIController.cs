using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIController : MonoBehaviour
{
    [Serializable]
    public class HPBar
    {
        public int activeHealthBars;
        public float hpBarUpperLimit;
        public float hpBarLowerLimit;
    }

    [SerializeField] private GameObject mainContainer;

    [SerializeField] private Image portraitContainer;
    [SerializeField] private Image livesBar;

    [SerializeField] private Sprite[] portraits;

    public static HealthUIController Instance;


    public enum Portraits
    {
        Full,
        Damaged,
        Dead
    }


    [SerializeField] private HPBar[] hpBars;

    public HPBar currentHPBar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InitUI(int currentHealth, int healthPerBar, int currentHealthBars)
    {
        UpdateCurrentHealthBar(currentHealthBars);
        float currentBarAmount = (currentHPBar.hpBarUpperLimit - currentHPBar.hpBarLowerLimit) * currentHealth / healthPerBar;
        livesBar.fillAmount = currentHPBar.hpBarLowerLimit + currentBarAmount;
    }

    public void UpdateHealthUI(int currentHealth, int healthPerBar, int currentHealthBars)
    {
        SetPortraitImage(Portraits.Damaged);
        float currentBarAmount = (currentHPBar.hpBarUpperLimit - currentHPBar.hpBarLowerLimit) * currentHealth / healthPerBar;
        livesBar.fillAmount = currentHPBar.hpBarLowerLimit + currentBarAmount;
        if (currentHealthBars == 3 && currentHealth == healthPerBar)
        {
            SetPortraitImage(Portraits.Full);
        }
    }


    public void UpdateCurrentHealthBar(int currentHealthBars)
    {
        currentHPBar = Array.Find(hpBars, hpBar => hpBar.activeHealthBars == currentHealthBars);
    }

    private void SetPortraitImage(Portraits portrait)
    {
        portraitContainer.sprite = portraits[(int)portrait];
    }

    public void SetDeadPortraitImage()
    {
        portraitContainer.sprite = portraits[(int)Portraits.Dead];
    }

    public void ResetHealthUI()
    {
        UpdateCurrentHealthBar(3);
        portraitContainer.sprite = portraits[(int)Portraits.Full];
        livesBar.fillAmount = 1;
    }

}
