using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public HorizontalLayoutGroup lifesLayout;
    public TextMeshProUGUI prismLabel;

    private LifeIcon[] lifeIcons
    {
        get
        {
            return lifesLayout.GetComponentsInChildren<LifeIcon>(true);
        }
    }

    private void Start()
    {
        SetLifes(GameManager.startingForm, GameManager.maxLifes);

        if (Player2DController.Instance != null)
        {
            Player2DController.OnLifesChangedEvent += SetLifes;
            Player2DController.OnPrismsChangedEvent += SetPrisms;
        }
    }

    public void SetLifes(FormState form, int lifes)
    {
        if (!this) return;
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i + 1 > GameManager.maxLifes)
            {
                lifeIcons[i].SetState(form, LifeIconState.Off);
            }
            else if (i < lifes)
            {
                lifeIcons[i].SetState(form, LifeIconState.Normal);
            }
            else
            {
                lifeIcons[i].SetState(form, LifeIconState.Damaged);
            }
        }
    }

    public void SetPrisms(int prisms)
    {
        if (!this) return;
        prismLabel.text = prisms.ToString();
    }
}
