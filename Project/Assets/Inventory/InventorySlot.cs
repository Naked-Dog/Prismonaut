[System.Serializable]
public class InventorySlot
{
    public string ItemID;
    public int Amount;

    public bool IsEmpty => string.IsNullOrEmpty(ItemID);

    public void Clear()
    {
        ItemID = null;
        Amount = 0;
    }
}
