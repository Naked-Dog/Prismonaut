using UnityEngine;
using UnityEngine.EventSystems;

public class OptionItemUI : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    [SerializeField] private GameObject selectionIcon;

    private void Awake()
    {
        if (selectionIcon != null)
            selectionIcon.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (selectionIcon != null)
            selectionIcon.SetActive(true);
        AudioManager.Instance.Play2DSound(MenuSoundsEnum.Scroll);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (selectionIcon != null)
            selectionIcon.SetActive(false);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        AudioManager.Instance?.Play2DSound(MenuSoundsEnum.Select);
    }
}
