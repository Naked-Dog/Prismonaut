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
        DialogueController.Instance.RunDialogue(ParseNarrative(narrativeText), endEvent);
        Debug.Log("Dialogue started");
        if (destroyOnInteract) Destroy(gameObject);
    }

    public void RunDialogue()
    {        
        Debug.Log("Run GA");
        DialogueController.Instance.RunDialogue(ParseNarrative(narrativeText), endEvent);
    }
}
