
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionItemUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private OptionsPanelUI soundPlayer;
    [SerializeField] private GameObject selectorObject;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Color selectedColor;
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
        //soundPlayer.menuAudio.PlayAudioClip("Scroll", false, 0.4f);
    }

    private void InitOptionItem(){
        if(!gameObject.GetComponent<Selectable>()){
            gameObject.AddComponent<Selectable>();
        }
    }

    private void DisplaySelectorObject(bool active){
        if(!selectorObject) return;
        selectorObject.SetActive(active);
        if(selectedColor != null){
            if(active == true) label.color = selectedColor;
            else label.color = Color.white;
        } 
    }

    public void ResetOptionItem(){
        gameObject.GetComponent<Selectable>().interactable = true;
    }

    public void playSelectSound(){
        //soundPlayer.menuAudio.PlayAudioClip("Select", false, 0.4f);
    }
}
