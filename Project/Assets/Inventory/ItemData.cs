using UnityEngine;

public enum ItemType
{
    Passive,
    Consumable,
    Currency,
    Key
}

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string ID;
    public string Name;

    [Header("Visuals")]
    public Sprite Sprite;

    [Header("Gameplay")]
    public ItemType Type;
    public bool Stackable = true;
    public int MaxStackSize = 99;
}
