using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> Items = new();

    private Dictionary<string, ItemData> m_lookup;

    private void OnEnable()
    {
        m_lookup = new Dictionary<string, ItemData>();

        foreach (var item in Items)
        {
            if (!m_lookup.ContainsKey(item.ID))
            {
                m_lookup.Add(item.ID, item);
            }
            else
            {
                Debug.LogError($"Duplicate item ID: {item.ID}");
            }
        }
    }

    public ItemData GetItem(string id)
    {
        m_lookup.TryGetValue(id, out var item);
        return item;
    }
}