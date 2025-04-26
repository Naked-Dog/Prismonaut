using PathFinder;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(PathfindingGraph2D))]
public class GridGraphEditor : Editor
{
    BoxBoundsHandle boxBoundsHandle = new BoxBoundsHandle();
    PathfindingGraph2D grid => (PathfindingGraph2D)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Scan"))
        {
            grid.Scan();
        }
    }

    void OnSceneGUI()
    {
        // 1) Centro del handle = grid.gridOrigin
        boxBoundsHandle.center = grid.gridOrigin;
        // 2) Tamaño del handle = grid.gridWorldSize
        boxBoundsHandle.size = new Vector3(
            grid.gridWorldSize.x,
            grid.gridWorldSize.y,
            0.1f
        );
        boxBoundsHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;

        Handles.color = Color.green;
        EditorGUI.BeginChangeCheck();
        boxBoundsHandle.DrawHandle();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(grid, "Resize/Move Grid");

            // 3) Actualiza TAMBIÉN el origen y el tamaño
            grid.gridWorldSize = new Vector2(
                boxBoundsHandle.size.x,
                boxBoundsHandle.size.y
            );
            grid.gridOrigin = boxBoundsHandle.center;

            grid.Scan();
            EditorUtility.SetDirty(grid);
        }
    }
}
