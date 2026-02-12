using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private string m_itemID;
    [SerializeField] private int m_amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        if (inventory.AddItem(m_itemID, m_amount))
        {
            Destroy(gameObject);
        }
    }
}
