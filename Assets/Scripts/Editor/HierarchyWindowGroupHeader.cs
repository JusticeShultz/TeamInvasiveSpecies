using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class HierarchyWindowGroupHeader
{
    static HierarchyWindowGroupHeader()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject != null && gameObject.name.StartsWith("---", System.StringComparison.Ordinal))
        {
            EditorGUI.DrawRect(selectionRect, new Color(0.2f, 0.5f, 0.5f, 1));
            EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace("-", "").ToUpperInvariant());
        }
        else
        if (gameObject != null && gameObject.name.StartsWith(">>>", System.StringComparison.Ordinal))
        {
            EditorGUI.DrawRect(selectionRect, new Color(0.7f, 0.2f, 0.2f, 1));
            EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace(">", "").ToUpperInvariant());
        }
        else
        if (gameObject != null && gameObject.name.StartsWith("!!!", System.StringComparison.Ordinal))
        {
            EditorGUI.DrawRect(selectionRect, new Color(0.3f, 0.4f, 0.5f, 1));
            EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace("!", "").ToUpperInvariant());
        }
        else
        if (gameObject != null && gameObject.name.StartsWith("^^^", System.StringComparison.Ordinal))
        {
            EditorGUI.DrawRect(selectionRect, new Color(0.6f, 0.6f, 0.3f, 1));
            EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace("^", "").ToUpperInvariant());
        }
    }
}