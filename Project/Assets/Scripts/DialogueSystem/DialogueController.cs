using System.Collections;
using UnityEngine;

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
    private Narrative currentNarrative;
    private int currentDialogueIndex = 0;
    private DialogueType currentType;
    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {

        if(isDialogueRunning){
            if(Input.GetKeyDown(KeyCode.Backspace))
            {
                EndDialogue();            
            }

            if(Input.GetKeyDown(KeyCode.Space) && currentType == DialogueType.Text)
            {
                if(currentDialogueComplete)
                {
                    RunNextDialogue();
                    return;
                }

                CompleteDialogue();
            }
        }
    }    

    public void RunDialogue(Narrative narrative) 
    {   
        currentNarrative = narrative;
        currentDialogueIndex = 0;
        isDialogueRunning = true;

        if(narrative.dialogues.Count != 0)
        {
            currentDialogueComplete =  false;
            currentType = DialogueType.Text;
            StartCoroutine(DialogueSequence(narrative.dialogues[currentDialogueIndex]));
        }
        else if(narrative.choiseNarrative)
        {
            currentType = DialogueType.Choices;
            viewController.SetDialoguePanel(currentType);
            viewController.DisplayChoices(narrative.choiseNarrative);
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

        if(currentDialogueIndex < currentNarrative.dialogues.Count)
        {
            currentDialogueComplete =  false;
            currentType = DialogueType.Text;
            StartCoroutine(DialogueSequence(currentNarrative.dialogues[currentDialogueIndex]));
        }
        else 
        {
            if(currentNarrative.choiseNarrative)
            {
                currentType = DialogueType.Choices;
                viewController.SetDialoguePanel(currentType);
                viewController.DisplayChoices(currentNarrative.choiseNarrative);
            }
        }
    }


    private IEnumerator DialogueSequence(Dialogue dialogue)
    {    
        currentActor = dialogue.actor;
        viewController.SetActor(currentActor);
        viewController.SetDialoguePanel(currentType);

        if(currentDialogueIndex == 0)
        {
            yield return viewController.OpenDialoguePanel();
        }

        yield return WriteDialogue(dialogue);

        viewController.ShowNextSign();
        currentDialogueComplete =  true;
        
    }

    private IEnumerator WriteDialogue(Dialogue dialogue)
    {
        currentDialogueText = dialogue.dialogueText;
        yield return StartCoroutine(WriteCharByChar(currentDialogueText, writeSpeed));
    }

    public IEnumerator WriteCharByChar( string dialogueText, float writeSpeed = 0.1f)
    {
        foreach(var character in dialogueText)
        {
            playDialogueSFX(viewController.dialogueTMPText.text);
            viewController.dialogueTMPText.text += character;
            yield return new WaitForSeconds(writeSpeed);
        }
    }

    public void playDialogueSFX(string text)
    {
        if(text.Length % currentActor.characterFrecuency == 0){
            audioSource.Stop();
            audioSource.pitch = Random.Range(currentActor.minPitch, currentActor.maxPitch);
            audioSource.PlayOneShot(currentActor.dialogueSFX);
        }
    }

    private void CompleteDialogue()
    {
        StopAllCoroutines();
        viewController.DisplayFullText(currentDialogueText);
        viewController.ShowNextSign();
        currentDialogueComplete = true;
    }

    public void EndDialogue(){
        StopAllCoroutines();
        StartCoroutine(viewController.CloseDialoguePanel());
        isDialogueRunning = false;
    }


}