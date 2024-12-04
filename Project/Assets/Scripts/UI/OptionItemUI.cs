
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionItemUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject selectorObject;
    private void Awake(){
        InitOptionItem();
    }

    public void OnSelect(BaseEventData eventData)
    {
        DisplaySelectorObject(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DisplaySelectorObject(false);
    }

    private void InitOptionItem(){
        if(!gameObject.GetComponent<Selectable>()){
            gameObject.AddComponent<Selectable>();
        }
    }

    private void DisplaySelectorObject(bool active){
        if(!selectorObject) return;
        selectorObject.SetActive(active);
    }

    public void ResetOptionItem(){
        gameObject.GetComponent<Selectable>().interactable = true;
    }
}
