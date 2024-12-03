using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsPanelUI : PanelUI
{
    [SerializeField] private Selectable initialSelectableObject;
    [SerializeField] private List<OptionItemUI> optionItems;
    
    private void OnEnable(){
        InitOptionItems();
        InitOptionsSelection();
    }

    private void InitOptionItems()
    {
       if(optionItems.Count == 0) return;
       foreach(OptionItemUI option in optionItems){
            option.ResetOptionItem();
       }
    }

    private void InitOptionsSelection(){
        if(!initialSelectableObject) return;
        if(!EventSystem.current) return;
        EventSystem.current.SetSelectedGameObject(initialSelectableObject.gameObject);
    }
}
