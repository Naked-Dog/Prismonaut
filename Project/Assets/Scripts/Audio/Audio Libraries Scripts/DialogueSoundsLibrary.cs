using UnityEngine;

public enum DialogueSoundsEnum
{
    Skip
}

[CreateAssetMenu(menuName = "Audio/Dialogue Sounds Library")]
public class DialogueSoundsLibrary : AudioLibrary<DialogueSoundsEnum> { }
