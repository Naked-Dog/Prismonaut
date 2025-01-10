using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // Esto es para trabajar con TextMeshPro.

public class ButtonEffects : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI buttonText; // Texto del botón.
    [SerializeField] private Color selectedColor = new Color(250 / 255f, 201 / 255f, 252 / 255f); // Color FAC9FC.
    [SerializeField] private Vector3 selectedPositionOffset = new Vector3(10, 0, 0); // Movimiento a la derecha.

    private Color originalColor; // Para guardar el color original.
    private Vector3 originalPosition; // Para guardar la posición original.

    private void Awake()
    {
        // Guarda el color y la posición originales del texto.
        originalColor = buttonText.color;
        originalPosition = buttonText.rectTransform.localPosition;
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Cambia el color y mueve el texto cuando se selecciona.
        buttonText.color = selectedColor;
        buttonText.rectTransform.localPosition = originalPosition + selectedPositionOffset;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // Restaura el color y la posición originales cuando se deselecciona.
        buttonText.color = originalColor;
        buttonText.rectTransform.localPosition = originalPosition;
    }
}
