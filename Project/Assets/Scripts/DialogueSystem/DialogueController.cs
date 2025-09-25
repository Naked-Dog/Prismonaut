using System;
using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;
using UnityEngine.Events;

public enum DialogueType
{
    Text,
    Choices
}

public class DialogueController : MonoBehaviour
{
    [SerializeField] private float writeSpeed = 0.1f;
    [SerializeField] private DialogueView viewController;
    [HideInInspector] public bool isDialogueRunning;

    private DialogueActor currentActor;
    private string currentDialogueText;
    private bool currentDialogueComplete;
    private Conversation currentConversation;
    private List<Dialogue> currentDialogues;
    private List<Choice> currentChoices;
    private int currentDialogueIndex = 0;
    private DialogueType currentType;
    private EventBus eventBus;
    private int currentCharactersCount = 0;
    private UnityEvent endEvent;

    public static DialogueController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetEventBus(EventBus bus)
    {
        eventBus = bus;
    }

    public void RunDialogue(Narrative narrative, UnityEvent endEvent = null)
    {
        eventBus.Publish(new RequestEnableDialogueInputs());
        currentDialogueIndex = 0;
        isDialogueRunning = true;

        List<Conversation> conversations = narrative.conversations;
        currentConversation = conversations[0];
        currentDialogues = currentConversation.dialogues;
        currentChoices = currentConversation.choices;

        if (endEvent != null)
        {
            this.endEvent = endEvent;
        }

        if (currentDialogues.Count != 0)
        {
            currentDialogueComplete = false;
            currentType = DialogueType.Text;
            StartCoroutine(DialogueSequence(currentDialogues[currentDialogueIndex]));
        }
        else if (currentChoices.Count != 0)
        {
            currentType = DialogueType.Choices;
            viewController.SetDialoguePanel(currentType);
            viewController.DisplayChoices(currentChoices);
        }
        else
        {
            Debug.LogError("Narrative is Empty");
            isDialogueRunning = false;
            return;
        }
    }

    private void RunNextDialogue()
    {
        currentDialogueIndex++;
        AudioManager.Instance.Play2DSound(DialogueSoundsEnum.Skip);

        if (currentDialogueIndex < currentDialogues.Count)
        {
            currentDialogueComplete = false;
            currentType = DialogueType.Text;
            StartCoroutine(DialogueSequence(currentDialogues[currentDialogueIndex]));
        }
        else
        {
            if (currentConversation.choices.Count != 0)
            {
                currentType = DialogueType.Choices;
                viewController.SetDialoguePanel(currentType);
                viewController.DisplayChoices(currentConversation.choices);
            }
            else
            {
                EndDialogue();
            }
        }
    }


    private IEnumerator DialogueSequence(Dialogue dialogue, bool resume = false)
    {
        currentActor = GetActor(dialogue.actor);
        viewController.SetActor(currentActor);
        viewController.SetDialoguePanel(currentType, resume);

        if (currentDialogueIndex == 0)
        {
            yield return viewController.OpenDialoguePanel();
        }

        yield return WriteDialogue(dialogue);

        viewController.ShowNextSign();
        currentDialogueComplete = true;
    }

    private IEnumerator WriteDialogue(Dialogue dialogue)
    {
        currentDialogueText = dialogue.text;
        yield return StartCoroutine(WriteCharByChar(currentDialogueText, writeSpeed));
    }

    public IEnumerator WriteCharByChar(string dialogueText, float writeSpeed = 0.1f)
    {
        viewController.dialogueTMPText.text = dialogueText;
        viewController.dialogueTMPText.maxVisibleCharacters = 0;
        int totalChars = dialogueText.Length;

        for (int i = currentCharactersCount; i < totalChars; i++)
        {
            char character = dialogueText[i];
            playDialogueSFX(character.ToString());
            viewController.dialogueTMPText.maxVisibleCharacters = i + 1;
            currentCharactersCount++;
            yield return new WaitForSeconds(writeSpeed);
        }
        currentCharactersCount = 0;
    }

    public void PauseDialogue()
    {
        if (isDialogueRunning) StopAllCoroutines();
    }

    public void ResumeDialogue()
    {
        if (!currentDialogueComplete)
        {
            StartCoroutine(WriteCharByChar(currentDialogues[currentDialogueIndex].text, writeSpeed));
        }
    }

    public void playDialogueSFX(string letter)
    {
        char c = letter[0];

        if (!char.IsLetter(c)) return;

        var upper = char.ToUpper(c);

        if (Enum.TryParse<AlphabetEnum>(upper.ToString(), out var key))
        {
            AudioManager.Instance.Play2DSoundByLibrary(
                key,
                currentActor.alphabetSoundsLibrary
            );
        }
    }

    private void CompleteDialogue()
    {
        StopAllCoroutines();
        viewController.dialogueTMPText.maxVisibleCharacters = viewController.dialogueTMPText.text.Length;
        viewController.ShowNextSign();
        currentDialogueComplete = true;
        currentCharactersCount = 0;
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(viewController.CloseDialoguePanel());
        isDialogueRunning = false;
        eventBus.Publish(new RequestDisableDialogueInputs());
        endEvent?.Invoke();
        endEvent = null;
    }

    private DialogueActor GetActor(string actorName)
    {
        var path = $"Actors/{actorName}";
        DialogueActor actor = Resources.Load<DialogueActor>(path);
        return actor;
    }

    public void SkipDialogue()
    {
        if (isDialogueRunning)
        {

            if (currentType == DialogueType.Text)
            {
                if (currentDialogueComplete)
                {
                    RunNextDialogue();
                    return;
                }

                CompleteDialogue();
            }
        }
    }
}