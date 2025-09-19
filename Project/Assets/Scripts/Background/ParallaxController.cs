using UnityEngine;
using UnityEngine.UI;

public class ParallaxController : MonoBehaviour
{
    // La cámara de tu escena. Asigna esto en el Inspector.
    private Camera playerCamera; 

    // Velocidad base del efecto de parallax.
    [Range(0.01f, 0.5f)]
    [SerializeField] private float parallaxSpeed = 0.1f;

    // Distancias Z de cada capa de fondo. Puedes ajustarlas en el Inspector.
    // Mayor distancia = menor movimiento de parallax.
    [SerializeField] private float[] layerDistances = { 10f, 20f, 50f, 100f }; 

    private Vector3 lastCameraPosition;

    // Almacenamos las imágenes para acceso rápido.
    private RawImage[] backgroundImages; 

    void Start()
    {
        playerCamera = Camera.main;
        // Guardamos la posición inicial de la cámara.
        lastCameraPosition = playerCamera.transform.position;
        
        // Obtenemos los componentes RawImage de los hijos.
        backgroundImages = GetComponentsInChildren<RawImage>();

        // Aseguramos que tenemos la misma cantidad de imágenes que de distancias.
        if (backgroundImages.Length != layerDistances.Length)
        {
            Debug.LogError("The number of background images and layer distances must match.");
            return;
        }

        // Asignamos una textura repetible por defecto si no tienen una.
        // Esto es útil para evitar errores si no asignaste una.
        for (int i = 0; i < backgroundImages.Length; i++)
        {
            if (backgroundImages[i].texture == null)
            {
                Debug.LogWarning("RawImage at index " + i + " has no texture. Assigning a default one.");
                // Puedes asignar una textura predeterminada si lo deseas.
            }
        }
    }

    void LateUpdate()
    {
        // Calculamos el movimiento de la cámara.
        Vector3 cameraDelta = playerCamera.transform.position - lastCameraPosition;
        
        // Recorremos cada imagen de fondo.
        for (int i = 0; i < backgroundImages.Length; i++)
        {
            // Calculamos la velocidad de parallax para esta capa.
            // Usamos la distancia Z de cada capa. Cuanto mayor sea la distancia, más lento se moverá.
            float currentParallaxSpeed = parallaxSpeed * (1 / layerDistances[i]);
            
            // Obtenemos el offset actual de la textura.
            Vector2 currentOffset = backgroundImages[i].uvRect.position;

            // Calculamos el nuevo offset.
            Vector2 newOffset = new Vector2(currentOffset.x + cameraDelta.x * currentParallaxSpeed, currentOffset.y);

            // Asignamos el nuevo offset a la textura.
            backgroundImages[i].uvRect = new Rect(newOffset.x, newOffset.y, backgroundImages[i].uvRect.width, backgroundImages[i].uvRect.height);
        }

        // Actualizamos la posición de la cámara para el próximo frame.
        lastCameraPosition = playerCamera.transform.position;
    }
}