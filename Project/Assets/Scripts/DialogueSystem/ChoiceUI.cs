using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI choiseText;
    
    public void SetChoise(Choice choise)
    {
        choiseText.text = choise.text;
        //Method to add the option selection action. It will be used when implementing options.
        //GetComponent<Button>().onClick.AddListener(()=> DialogueController.Instance.RunDialogue(choise.nextNarrative));
    }
}
