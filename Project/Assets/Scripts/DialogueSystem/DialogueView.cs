
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{

    [Header("Dialogue Panel")]
    public TextMeshProUGUI dialogueTMPText;
    [SerializeField] private GameObject dialoguePanelContainer;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Image portraitContainer;
    [SerializeField] private GameObject nextDialogueSing;

    [Header("Choises")]
    [SerializeField] private GameObject choisesContainer;
    [SerializeField] private GameObject choisesPrefab;

    private RectTransform panelRecTransform => dialoguePanelContainer.GetComponent<RectTransform>();

    public void SetDialoguePanel(DialogueType type) 
    {
        switch(type)
        {
            case DialogueType.Text:
                dialogueTMPText.gameObject.SetActive(true);
                dialogueTMPText.text = "";
                portraitContainer.gameObject.SetActive(true);
                choisesContainer.SetActive(false);
                ShowNextSign(false);
                foreach(Transform child in choisesContainer.transform)
                {
                    Destroy(child.gameObject);
                }
                choisesContainer.SetActive(false);
                break;

            case DialogueType.Choices:
                choisesContainer.SetActive(true);
                dialogueTMPText.gameObject.SetActive(false);
                portraitContainer.gameObject.SetActive(false);
                ShowNextSign(false);
                break;

            default:
                break;
        }
    }

    public void SetActor(DialogueActor actor)
    {   
        portraitImage.sprite = actor.portrait;
        dialogueTMPText.font = actor.dialogueFont;
        dialogueTMPText.color = actor.fontColor;
    }

    public void DisplayChoices(ChoiceNarrative choiceNarrative)
    {
        foreach(Choice choise in choiceNarrative.choices)
        {
            GameObject choiseObject = Instantiate(choisesPrefab);
            var parentRecTransform = choisesContainer.GetComponent<RectTransform>();
            choiseObject.GetComponent<RectTransform>().SetParent(parentRecTransform, false);
            choiseObject.GetComponent<ChoiceUI>().SetChoise(choise);
        }

        EventSystem.current.SetSelectedGameObject(choisesContainer.transform.GetChild(0).gameObject);
        SetDialoguePanel(DialogueType.Choices);
    }

    public void ShowNextSign(bool active = true)
    {
        nextDialogueSing.SetActive(active);
    }

    public IEnumerator OpenDialoguePanel()
    {
        dialoguePanelContainer.gameObject.SetActive(true);
        Tween openTween = panelRecTransform.DOScale(Vector3.one, 0.3f);
        yield return openTween.WaitForCompletion();
    }

    public IEnumerator CloseDialoguePanel()
    {
        Tween closeTween = panelRecTransform.DOScale(Vector3.zero, 0.3f);
        yield return closeTween.WaitForCompletion();

        dialoguePanelContainer.SetActive(false);
    }

    public void DisplayFullText(String fullText)
    {
        dialogueTMPText.text = fullText;
    }
}
