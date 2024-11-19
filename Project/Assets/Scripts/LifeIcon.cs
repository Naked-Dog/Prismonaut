using UnityEngine;
using UnityEngine.UI;

public class LifeIcon : MonoBehaviour
{
    public Sprite normalCircleSprite;
    public Sprite normalSquareSprite;
    public Sprite normalTriangleSprite;
    public Sprite damagedCircleSprite;
    public Sprite damagedSquareSprite;
    public Sprite damagedTriangleSprite;
    public Image imageRenderer;

    private LifeIconState state = LifeIconState.Undefined;
    private FormState form = FormState.Undefined;


    public void SetState(FormState newForm, LifeIconState newState)
    {
        if (newState == LifeIconState.Off)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        switch (newForm)
        {
            case FormState.Circle:
                imageRenderer.sprite = (newState == LifeIconState.Damaged) ? damagedCircleSprite : normalCircleSprite;
                imageRenderer.color = (newState == LifeIconState.Damaged) ? Color.gray : Color.white;
                break;
            case FormState.Square:
                imageRenderer.sprite = (newState == LifeIconState.Damaged) ? damagedSquareSprite : normalSquareSprite;
                imageRenderer.color = (newState == LifeIconState.Damaged) ? Color.gray : Color.white;
                break;
            case FormState.Triangle:
                imageRenderer.sprite = (newState == LifeIconState.Damaged) ? damagedTriangleSprite : normalTriangleSprite;
                imageRenderer.color = (newState == LifeIconState.Damaged) ? Color.gray : Color.white;
                break;
        }
        state = newState;
        form = newForm;
    }
}
