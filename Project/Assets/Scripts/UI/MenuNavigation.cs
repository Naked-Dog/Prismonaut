using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private InputActionReference navigateAction;
    [SerializeField] private InputActionReference submitAction;
    [SerializeField] private InputActionReference cancelAction;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private int startIndex = 0;

    private int currentIndex;
    private bool canMove = true;
    private float moveCooldown = 0.15f;

    private void OnEnable()
    {
        currentIndex = startIndex;
        SelectButton(currentIndex);

        navigateAction.action.performed += OnNavigate;
        submitAction.action.performed += OnSubmit;
        cancelAction.action.performed += OnCancel;
    }

    private void OnDisable()
    {
        navigateAction.action.performed -= OnNavigate;
        submitAction.action.performed -= OnSubmit;
        cancelAction.action.performed -= OnCancel;
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (!canMove) return;

        Vector2 input = ctx.ReadValue<Vector2>();
        int dir = 0;

        if (input.y > 0.5f) dir = -1;
        if (input.y < -0.5f) dir = 1;

        if (dir != 0)
        {
            currentIndex = (currentIndex + dir + buttons.Length) % buttons.Length;
            SelectButton(currentIndex);
            StartCoroutine(MoveCooldown());
        }
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        var btn = buttons[currentIndex].GetComponent<Button>();
        btn?.onClick.Invoke();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        var panel = GetComponentInParent<PanelBase>();
        if (panel != null)
            panel.SendMessage("OnCancel", SendMessageOptions.DontRequireReceiver);
    }

    private void SelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttons[index]);
    }

    private System.Collections.IEnumerator MoveCooldown()
    {
        canMove = false;
        yield return new WaitForSecondsRealtime(moveCooldown);
        canMove = true;
    }
}
