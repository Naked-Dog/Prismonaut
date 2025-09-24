using System.Collections.Generic;
using UnityEngine;

public class GlobalConstants
{
    private static Dictionary<System.Type, ScriptableObject> constants = new();

    static GlobalConstants()
    {
        var loadedConstants = Resources.LoadAll<ScriptableObject>("Constants");
        foreach (var constant in loadedConstants)
        {
            constants[constant.GetType()] = constant;
        }
    }

    public static T Get<T>() where T : ScriptableObject
    {
        if (constants.TryGetValue(typeof(T), out var obj))
        {
            return obj as T;
        }

        Debug.LogError($"GlobalConstants: No constant of type {typeof(T)} found.");
        return null;
    }
}