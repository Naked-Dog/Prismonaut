using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Choice
{
    [TextArea] public string choiceText;
    public Narrative nextNarrative;
}

//temporal option to write the Choices in the narrative.
[CreateAssetMenu(fileName = "NewChoiceNarrative", menuName = "DialogueSystem/NewChoiceNarrative", order = 3)]
public class ChoiceNarrative : ScriptableObject
{
    public List<Choice> choices = new();
}