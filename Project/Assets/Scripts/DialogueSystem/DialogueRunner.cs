using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private Narrative narrative;
    [SerializeField] private Player2DController player;
    [SerializeField] private bool isDialoguePlayed;

    protected void Update(){
        if(player && !isDialoguePlayed && !DialogueController.Instance.IsDialogueOpen())
        {
            DialogueController.Instance.RunDialogue(narrative);
            isDialoguePlayed = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        player = other.GetComponent<Player2DController>();
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.GetComponent<Player2DController>()){
            player = null;
            isDialoguePlayed = false;
        } 
            
    }
}
