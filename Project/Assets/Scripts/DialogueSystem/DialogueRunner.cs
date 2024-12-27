using PlayerSystem;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private Narrative narrative;
    [SerializeField] private PlayerBaseModule player;
    [SerializeField] private bool isDialoguePlayed;

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
}
