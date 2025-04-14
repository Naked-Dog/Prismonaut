using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentProperties
{
    public float maxClimbHeight = 0.5f;

    //LayerMask used for validate collision with obstacle during the path generation. Gaaaaaaaaa
    public LayerMask groundLayer;
}

[RequireComponent(typeof(Rigidbody2D))]
public class PathFinder2DAgent : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float repathRate = 1f;
    public GridGraph gridGraph;

    [Header("Destino")]
    [SerializeField] private Transform destination;

    [Header("Propiedades del Agente")]
    public AgentProperties agentProperties; // Contiene groundLayer, maxClimbHeight, etc.

    [SerializeField] private Collider2D col;
    [SerializeField] private float groundOffset = 0.05f;

    private Rigidbody2D rb;
    private Pathfinder pathfinder;
    private List<Vector3> currentPath;
    private int pathIndex;
    private float repathTimer;

    public Vector3 TargetDestination
    {
        get => destination ? destination.position : Vector3.zero;
        set
        {
            if (destination == null)
            {
                GameObject aux = new GameObject("PathFinder2DAgent_Destination");
                aux.transform.position = value;
                destination = aux.transform;
            }
            else
            {
                destination.position = value;
            }
            RecalculatePath();
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (gridGraph != null && gridGraph.nodeMap.Count == 0)
        {
            gridGraph.Scan();
        }
        pathfinder = new Pathfinder(gridGraph.nodeMap);
        RecalculatePath();
    }

    private void Update()
    {
        repathTimer += Time.deltaTime;
        if (repathTimer >= repathRate)
        {
            repathTimer = 0f;
            RecalculatePath();
        }
    }

    private void FixedUpdate()
    {
        if (currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
            return;

        // Usamos el siguiente nodo de la ruta y lo "proyectamos" al suelo.
        Vector3 targetNodePos = currentPath[pathIndex];
        Vector3 projectedTarget = GetGroundPositionFromPoint(targetNodePos);
        
        // Aquí podemos decidir mover horizontalmente y/o ajustar en Y.
        // Por ejemplo, mover horizontalmente hacia la posición "proyectada"
        Vector2 direction = ((Vector2)projectedTarget - rb.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(rb.position, projectedTarget) < 0.1f)
        {
            pathIndex++;
        }
    }

    /// <summary>
    /// Lanza un raycast desde un punto ligeramente elevado hacia abajo para obtener la posición exacta del suelo.
    /// </summary>
    /// <param name="point">La posición de referencia (del nodo)</param>
    /// <returns>La posición en la que se encuentra el suelo, o el punto original si no se detecta colisión.</returns>
    private Vector3 GetGroundPositionFromPoint(Vector3 point)
    {
        // Modifica el offset si es necesario para que el raycast no se inicie ya dentro del suelo.
        Vector3 rayOrigin = point + Vector3.up * 0.5f;
        // Longitud del raycast: ajusta según la altura máxima esperada.
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 5f, agentProperties.groundLayer);
        if (hit.collider != null)
        {
            // Aquí puedes ajustar con el groundOffset si es necesario para que el enemigo quede justo "pegado"
            return new Vector3(point.x, hit.point.y + (col.bounds.extents.y - groundOffset), point.z);
        }
        return point;
    }

    /// <summary>
    /// Obtiene la posición de los pies del agente
    /// </summary>
    private Vector3 GetBottomPosition()
    {
        return col.bounds.center + Vector3.down * (col.bounds.extents.y - groundOffset);
    }

    public void RecalculatePath()
    {
        if (destination == null || gridGraph == null || gridGraph.nodeMap.Count == 0)
            return;

        Vector3 start = GetBottomPosition();
        Vector3 end = destination.position;

        pathfinder = new Pathfinder(gridGraph.nodeMap);
        currentPath = pathfinder.FindPath(start, end, agentProperties);
        pathIndex = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (currentPath == null || currentPath.Count == 0)
            return;

        Gizmos.color = Color.yellow;
        foreach (var pos in currentPath)
        {
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }
}