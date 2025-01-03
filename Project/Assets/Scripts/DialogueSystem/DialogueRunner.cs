using PlayerSystem;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private TextAsset narrativeText;
    [SerializeField] private PlayerBaseModule player;
    [SerializeField] private bool isDialoguePlayed;
    
    private Narrative narrative;

    protected void Start()
    {
        ParseNarrative(narrativeText);
    }

    protected void Update(){
        if(player && !isDialoguePlayed && !DialogueController.Instance.isDialogueRunning)
        {
            DialogueController.Instance.RunDialogue(narrative);
            isDialoguePlayed = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        player = other.GetComponent<PlayerBaseModule>();
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.GetComponent<PlayerBaseModule>()){
            player = null;
            isDialoguePlayed = false;
        } 
            
    }

    private void ParseNarrative(TextAsset narrativeJSON)
    {
        narrative = JsonUtility.FromJson<Narrative>(narrativeJSON.ToString());
    }
}
