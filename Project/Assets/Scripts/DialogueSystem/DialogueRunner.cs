using PlayerSystem;
using UnityEngine;

public class DialogueRunner : MonoBehaviour, IInteractable
{
    [SerializeField] private TextAsset narrativeText;
    public bool IsInteractable => !DialogueController.Instance.isDialogueRunning;


    private Narrative ParseNarrative(TextAsset narrativeJSON)
    {
        Narrative parseNarrative = JsonUtility.FromJson<Narrative>(narrativeJSON.ToString());
        return parseNarrative;
    }

    public void Interact()
    {
        DialogueController.Instance.RunDialogue(ParseNarrative(narrativeText));
    }
}
