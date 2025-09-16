using UnityEngine;
using DG.Tweening;

public class FormsUI : MonoBehaviour
{
    public float rotateDuration = 0.5f;
    public float scaleDuration = 0.5f;
    public RectTransform circle;
    public RectTransform square;
    public RectTransform triangle;

    private RectTransform[] shapes
    {
        get
        {
            return new[] { circle, square, triangle };
        }
    }
    private FormState form = FormState.Undefined;

    private void Start()
    {
        formChange(FormState.Circle, false);

        if (Player2DController.Instance != null)
        {
            Player2DController.OnFormChangedEvent += formChange;
        }
    }

    private void formChange(FormState newForm, bool isAnimated = true)
    {
        if (!this) return;
        if (form == newForm) return;

        for (int i = 0; i < shapes.Length; i++)
        {
            if (isAnimated)
            {
                shapes[i].DOScale(Vector3.one * ((FormState)i == newForm ? 1.5f : 1f), scaleDuration);
            }
            else
            {
                shapes[i].localScale = Vector3.one * ((FormState)i == newForm ? 1.5f : 1f);
            }
        }

        form = newForm;
    }
}
