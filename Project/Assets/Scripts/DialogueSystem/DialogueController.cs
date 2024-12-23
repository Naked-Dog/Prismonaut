using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private RectTransform dialoguePanel;
    [SerializeField] private Image dialogueBubble;
    [SerializeField] private Image actorPortrait;
    [SerializeField] private TextMeshProUGUI bubbleText;
    [SerializeField] private GameObject nextSing;
    [SerializeField] private RectTransform choisesContainer;
    [SerializeField] private GameObject choiseUIObject;
    [SerializeField] private float writeSpeed = 0.1f;

    private string currentDialogueText;
    private Coroutine writingCoroutine;
    private bool isDialogueRunning;
    private bool goNextDialogue;
    private bool isDialoguePanelOpen;

    public static DialogueController Instance {get; private set;}

    private void Awake(){
        if(Instance != null)
        {
            Destroy(gameObject);
        } 
        else 
        {
            Instance = this;
        }
    }

    private void Start()
    {
        dialoguePanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        //When close the dialogue?????
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            if(IsDialogueOpen()) EndDialogue();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!isDialogueRunning && !goNextDialogue)
            {
                goNextDialogue = true;
                return;
            }

            SkipDialogue();
        }
    }    

    public void RunDialogue(Narrative narrative) 
    {   
        StartCoroutine(NarrativeSequence(narrative));
    }


    private void SetDialogueBubble(DialogueActor actor)
    {
        actorPortrait.sprite = actor.portrait;
        actorPortrait.gameObject.SetActive(true);

        bubbleText.gameObject.SetActive(true);
        bubbleText.font = actor.dialogueFont;
        bubbleText.text = "";
        bubbleText.color = actor.fontColor;
        nextSing.SetActive(false);

        choisesContainer.gameObject.SetActive(false);
        foreach(Transform child in choisesContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void DisplayChoices(ChoiceNarrative choiceNarrative)
    {
        foreach(Choice choise in choiceNarrative.choices)
        {
            GameObject choiseObject = Instantiate(choiseUIObject);
            choiseObject.GetComponent<RectTransform>().SetParent(choisesContainer, false);
            choiseObject.GetComponent<ChoiceUI>().SetChoise(choise);
        }


        EventSystem.current.SetSelectedGameObject(choisesContainer.GetChild(0).gameObject);

        bubbleText.gameObject.SetActive(false);
        nextSing.SetActive(false);
        actorPortrait.gameObject.SetActive(false);

        choisesContainer.gameObject.SetActive(true);
    }

    private IEnumerator NarrativeSequence(Narrative narrative)
    {
        SetDialogueBubble(narrative.dialogues[0].actor);
        yield return DisplayDialoguePanel(true);
        
        foreach(Dialogue dialogue in narrative.dialogues){
            SetDialogueBubble(dialogue.actor);
            yield return DialogueSequence(dialogue);
        }

        if(narrative.choiseNarrative) DisplayChoices(narrative.choiseNarrative);
    }

    private IEnumerator DialogueSequence(Dialogue currentDialogue)
    {
        isDialogueRunning = true;
        goNextDialogue = false;
        nextSing.SetActive(false);

        currentDialogueText = currentDialogue.dialogueText;
        writingCoroutine = StartCoroutine(WriteDialogueText(currentDialogueText));
        
        while(writingCoroutine != null)
        {
            yield return null;
        }

        isDialogueRunning = false;
        
        nextSing.SetActive(true);

        while(!goNextDialogue)
        {
            yield return null;
        }
    }

    private IEnumerator DisplayDialoguePanel(bool open = false)
    {
        Tween bubbleTween;

        if(open)
        {
            if(isDialoguePanelOpen) yield break;

            isDialoguePanelOpen = true;
            dialoguePanel.localScale = Vector3.zero;
            dialoguePanel.gameObject.SetActive(true);
            bubbleTween = dialoguePanel.DOScale(Vector3.one, 0.3f);
        }
        else 
        {
            bubbleTween = dialoguePanel.DOScale(Vector3.zero, 0.3f);
            dialoguePanel.gameObject.SetActive(false);
            isDialoguePanelOpen = false;
        }

        yield return bubbleTween.WaitForCompletion();
    }

    private IEnumerator WriteDialogueText( string dialogueText)
    {
        foreach(var character in dialogueText)
        {
            bubbleText.text += character;
            yield return new WaitForSeconds(writeSpeed);
        }

        writingCoroutine = null;
    }

    public void EndDialogue(){
        StopAllCoroutines();
        writingCoroutine = null;
        StartCoroutine(DisplayDialoguePanel());
    }

    private void SkipDialogue()
    {
        if(writingCoroutine == null) return;
        StopCoroutine(writingCoroutine);
        writingCoroutine = null;
        bubbleText.text = currentDialogueText;
    }

    public bool IsDialogueOpen(){ return isDialoguePanelOpen;}
}
