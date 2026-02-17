using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public class UIControllerBase : MonoBehaviour
{
    [Header("Panel Management")]
    [SerializeField] private PanelBase m_startPanel;
    [SerializeField] private List<PanelBase> m_allPanels = new();

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;

    private PanelBase m_currentPanel;
    private InputAction m_cancelAction;

    public event Action<PanelBase> OnPanelChanged;

    protected virtual void Awake()
    {
        foreach (var p in m_allPanels)
        {
            if (p != null && p != m_startPanel)
                p.HideInstant();
        }
    }

    protected virtual void Start()
    {
        if (m_startPanel != null)
        {
            m_currentPanel = m_startPanel;
            m_currentPanel.Open();
            OnPanelChanged?.Invoke(m_currentPanel);
        }

        var uiMap = inputActions.FindActionMap("UI", true);
        m_cancelAction = uiMap.FindAction("Cancel", true);
        m_cancelAction.performed += OnCancel;
        m_cancelAction.Enable();
    }

    protected virtual void OnDestroy()
    {
        if (m_cancelAction != null)
        {
            m_cancelAction.performed -= OnCancel;
            m_cancelAction.Disable();
        }
    }

    protected virtual void OnCancel(InputAction.CallbackContext ctx)
    {
        if (m_currentPanel == null || m_currentPanel == m_startPanel) return;
        BackToStart();
    }

    public void OpenPanel(PanelBase newPanel)
    {
        if (newPanel == m_currentPanel) return;

        if (m_currentPanel != null)
        {
            m_currentPanel.Close();
        }

        newPanel.Open();
        m_currentPanel = newPanel;
        OnPanelChanged?.Invoke(m_currentPanel);
    }

    public void BackToStart()
    {
        if (m_currentPanel != null && m_currentPanel != m_startPanel)
        {
            m_currentPanel.Close();
        }

        if (m_startPanel != null)
        {
            m_startPanel.Open();
            m_currentPanel = m_startPanel;
            OnPanelChanged?.Invoke(m_currentPanel);
        }
    }

    public void ClearCurrentPanel() => m_currentPanel = null;

    public PanelBase GetCurrentPanel() => m_currentPanel;

    public IReadOnlyList<PanelBase> AllPanels => m_allPanels;
    public PanelBase StartPanel => m_startPanel;
    public InputActionAsset InputActions => inputActions;
}
