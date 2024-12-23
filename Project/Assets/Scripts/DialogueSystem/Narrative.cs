using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogue
{
    [TextArea] public string dialogueText;
    public DialogueActor actor; 
}

//temporal option to write the narrative.
[CreateAssetMenu(fileName = "NewNarrative", menuName = "DialogueSystem/NewNarrative", order = 2)]
public class Narrative : ScriptableObject
{
    public List<Dialogue> dialogues = new();
    public ChoiceNarrative choiseNarrative;
}




