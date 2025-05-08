using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CameraSystem;
using PlayerSystem;
using UnityEditor;
using UnityEngine;
using CameraSystem;

public enum DialogueType
{
    Text,
    Choices
}

public class DialogueController : MonoBehaviour
{
    [SerializeField] private float writeSpeed = 0.1f;
    [SerializeField] private DialogueView viewController;
    [SerializeField] private AudioClip skipSound;
    [HideInInspector] public bool isDialogueRunning;

    private DialogueActor currentActor;
    private string currentDialogueText;
    private bool currentDialogueComplete;
    private Conversation currentConversation;
    private List<Dialogue> currentDialogues;
    private List<Choice> currentChoices;
    private int currentDialogueIndex = 0;
    private DialogueType currentType;
    private AudioSource audioSource;
    private string[] ignoreChars = { " ", ",", "-", "_", "." };

    private EventBus eventBus;

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

        audioSource = GetComponent<AudioSource>();
    }

    public void SetEventBus(EventBus bus)
    {
        eventBus = bus;
    }

    public void RunDialogue(Narrative narrative)
    {
        eventBus.Publish(new RequestEnableDialogueInputs());
        CameraManager.Instance.ChangeCamera(CameraManager.Instance.SearchCamera(CineCameraType.Dialogue));
        currentDialogueIndex = 0;
        isDialogueRunning = true;

        List<Conversation> conversations = narrative.conversations;
        currentConversation = conversations[0];
        currentDialogues = currentConversation.dialogues;
        currentChoices = currentConversation.choices;


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


    private IEnumerator DialogueSequence(Dialogue dialogue)
    {
        currentActor = GetActor(dialogue.actor);
        viewController.SetActor(currentActor);
        viewController.SetDialoguePanel(currentType);

        if (currentDialogueIndex == 0)
        {
            yield return viewController.OpenDialoguePanel();
        }

        yield return WriteDialogue(dialogue);

        viewController.ShowNextSign();
        audioSource.PlayOneShot(skipSound);
        currentDialogueComplete = true;

    }

    private IEnumerator WriteDialogue(Dialogue dialogue)
    {
        currentDialogueText = dialogue.text;
        yield return StartCoroutine(WriteCharByChar(currentDialogueText, writeSpeed));
    }

    public IEnumerator WriteCharByChar(string dialogueText, float writeSpeed = 0.1f)
    {
        foreach (var character in dialogueText)
        {
            playDialogueSFX(character.ToString());
            viewController.dialogueTMPText.text += character;
            yield return new WaitForSeconds(writeSpeed);
        }
    }

    public void playDialogueSFX(string letter)
    {
        foreach (string character in ignoreChars)
        {
            if (character == letter)
            {
                return;
            }
        }

        var upper = letter.ToUpper();

        foreach (AudioClip clip in currentActor.alphabetSounds)
        {
            var dialogueLetter = clip.name.Last();
            if (dialogueLetter.ToString() == upper)
            {
                audioSource.PlayOneShot(clip);
                return;
            }
        }
    }

    private void CompleteDialogue()
    {
        StopAllCoroutines();
        viewController.DisplayFullText(currentDialogueText);
        viewController.ShowNextSign();
        audioSource.PlayOneShot(skipSound);
        currentDialogueComplete = true;
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(viewController.CloseDialoguePanel());
        isDialogueRunning = false;
        CameraManager.Instance.ChangeCamera(CameraManager.Instance.SearchCamera(CineCameraType.Regular));
        eventBus.Publish(new RequestDisableDialogueInputs());
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