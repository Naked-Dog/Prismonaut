using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceUI : MonoBehaviour
{
    [HideInInspector] public Choice choise;
    [SerializeField] private TextMeshProUGUI choiseText;

    public void SetChoise(Choice choise)
    {
        this.choise = choise;
        choiseText.text = choise.choiceText;
        GetComponent<Button>().onClick.AddListener(()=> DialogueController.Instance.RunDialogue(choise.nextNarrative));
    }
}
