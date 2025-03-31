using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGraph))]
public class GridGraphEditor : Editor
{
    GridGraph grid => (GridGraph)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Scan")){
            grid.Scan();
        }
    }
}
