using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewActor", menuName = "DialogueSystem/NewDialogueActor", order = 1)]
public class DialogueActor : ScriptableObject
{
    public Sprite portrait;
    public TMP_FontAsset dialogueFont;
    public Color fontColor = Color.white;
    public List<AudioClip> alphabetSounds;
}
