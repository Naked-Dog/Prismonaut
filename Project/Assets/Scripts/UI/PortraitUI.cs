using UnityEngine;
using UnityEngine.UI;

public class PortraitUI : MonoBehaviour
{
    public Image portraitRenderer;
    public Sprite circlePortrait;
    public Sprite squarePortrait;
    public Sprite trianglePortrait;

    public void SetForm(FormState newForm, bool isAnimated = true)
    {
        if (!this) return;
        switch (newForm)
        {
            case FormState.Circle:
                portraitRenderer.sprite = circlePortrait;
                break;
            case FormState.Square:
                portraitRenderer.sprite = squarePortrait;
                break;
            case FormState.Triangle:
                portraitRenderer.sprite = trianglePortrait;
                break;
        }
    }

    private void Start()
    {
        Player2DController.OnFormChangedEvent += SetForm;
    }
}
