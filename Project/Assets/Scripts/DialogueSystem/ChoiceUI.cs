using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI choiseText;
    
    public void SetChoise(Choice choise)
    {
        choiseText.text = choise.choiceText;
        GetComponent<Button>().onClick.AddListener(()=> DialogueController.Instance.RunDialogue(choise.nextNarrative));
    }
}
