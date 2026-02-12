using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_panel;
    [SerializeField] private PlayerInventory m_inventory;
    [SerializeField] private ItemDatabase m_itemDatabase;

    [Header("UI")]
    [SerializeField] private InventorySlotUI m_slotPrefab;
    [SerializeField] private Transform m_slotsParent;

    private InventorySlotUI[] m_slotsUI;
    private bool m_isOpen;

    private void Start()
    {
        if (m_itemDatabase == null)
        {
            Debug.LogError("ItemDatabase not assigned in InventoryUI");
            return;
        }
        m_panel.SetActive(false);

        m_slotsUI = new InventorySlotUI[m_inventory.Slots.Count];

        for (int i = 0; i < m_slotsUI.Length; i++)
        {
            m_slotsUI[i] = Instantiate(m_slotPrefab, m_slotsParent);
        }
        m_inventory.OnInventoryChanged += Refresh;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        m_isOpen = !m_isOpen;
        m_panel.SetActive(m_isOpen);

        if (m_isOpen)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        for (int i = 0; i < m_slotsUI.Length; i++)
        {
            m_slotsUI[i].Refresh(m_inventory.Slots[i], m_itemDatabase);
        }
    }

    private void OnDestroy()
    {
        if (m_inventory != null)
        {
            m_inventory.OnInventoryChanged -= Refresh;
        }
    }
}