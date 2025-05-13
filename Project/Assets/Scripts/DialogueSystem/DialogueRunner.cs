using PlayerSystem;
using UnityEngine;
using UnityEngine.Events;

public class DialogueRunner : MonoBehaviour, IInteractable
{
    [SerializeField] private TextAsset narrativeText;
    [SerializeField] private bool interactOnEnter;
    [SerializeField] private bool destroyOnInteract;
    [SerializeField] private UnityEvent endEvent;

    public bool IsInteractable => !DialogueController.Instance.isDialogueRunning;
    public bool InteractOnEnter => interactOnEnter;
    public bool DestroyOnInteract => destroyOnInteract;

    private Narrative ParseNarrative(TextAsset narrativeJSON)
    {
        Narrative parseNarrative = JsonUtility.FromJson<Narrative>(narrativeJSON.ToString());
        return parseNarrative;
    }

    public void Interact()
    {
        if(!gameObject) return;
        DialogueController.Instance.RunDialogue(ParseNarrative(narrativeText), endEvent);
        if (destroyOnInteract) Destroy(gameObject);
    }

    public void RunDialogue()
    {        
        DialogueController.Instance.RunDialogue(ParseNarrative(narrativeText), endEvent);
    }
}
