using PlayerSystem;
using UnityEngine;

public class DialogueRunner : MonoBehaviour, IInteractable
{
    [SerializeField] private TextAsset narrativeText;
    [SerializeField] private bool interactOnEnter;
    [SerializeField] private bool destroyOnInteract;

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
        DialogueController.Instance.RunDialogue(ParseNarrative(narrativeText));
        if (destroyOnInteract) Destroy(gameObject);
    }
}
