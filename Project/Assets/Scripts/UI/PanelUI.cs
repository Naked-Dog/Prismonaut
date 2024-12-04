using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PanelUI : MonoBehaviour, ICancelHandler
{
    [SerializeField]
    private UnityEvent OnPanelCancel = new UnityEvent();

    public void OnCancel(BaseEventData eventData)
    {
        OnPanelCancel.Invoke();
    }
}
