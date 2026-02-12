using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private TextMeshProUGUI m_amountText;

    public void Refresh(InventorySlot slot, ItemDatabase database)
    {
        if (slot.IsEmpty)
        {
            m_icon.enabled = false;
            m_amountText.text = "";
            return;
        }

        ItemData item = database.GetItem(slot.ItemID);

        if (item == null)
        {
            m_icon.enabled = false;
            m_amountText.text = "";
            Debug.LogWarning($"Item ID not found: {slot.ItemID}");
            return;
        }

        m_icon.enabled = true;
        m_icon.sprite = item.Sprite;
        m_amountText.text = slot.Amount > 1 ? slot.Amount.ToString() : "";
    }
}
