using UnityEngine;

public enum MenuSoundsEnum
{
    Scroll,
    Select
}

[CreateAssetMenu(menuName = "Audio/Menu Sounds Library")]
public class MenuSoundsLibrary : AudioLibrary<MenuSoundsEnum> { }
