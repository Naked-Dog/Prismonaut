using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewActor", menuName = "DialogueSystem/NewDialogueActor", order = 1)]
public class DialogueActor : ScriptableObject
{
    public Sprite portrait;
    public TMP_FontAsset dialogueFont;
    public Color fontColor = Color.white;
    public AudioClip dialogueSFX;
    public int characterFrecuency;

    [Range(0,1)]
    public float minPitch = -0.3f;

    [Range(1,1.5f)]
    public float maxPitch = 0.3f;
}
