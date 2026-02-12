using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int m_capacity = 20;
    [SerializeField] private ItemDatabase m_itemDatabase;

    public event Action OnInventoryChanged;

    private List<InventorySlot> m_slots = new();

    public IReadOnlyList<InventorySlot> Slots => m_slots;

    private void Awake()
    {
        for (int i = 0; i < m_capacity; i++)
        {
            m_slots.Add(new InventorySlot());
        }
    }

    public bool AddItem(string itemID, int amount = 1)
    {
        if (m_itemDatabase == null)
        {
            Debug.LogError("ItemDatabase not assigned on PlayerInventory");
            return false;
        }

        ItemData item = m_itemDatabase.GetItem(itemID);
        if (item == null)
        {
            Debug.LogWarning($"Item not found in database: {itemID}");
            return false;
        }

        if (item.Stackable)
        {
            foreach (var slot in m_slots)
            {
                if (slot.ItemID == itemID && slot.Amount < item.MaxStackSize)
                {
                    int spaceLeft = item.MaxStackSize - slot.Amount;
                    int toAdd = Mathf.Min(spaceLeft, amount);

                    slot.Amount += toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }
        }

        foreach (var slot in m_slots)
        {
            if (slot.IsEmpty)
            {
                slot.ItemID = itemID;
                slot.Amount = amount;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false;
    }
}