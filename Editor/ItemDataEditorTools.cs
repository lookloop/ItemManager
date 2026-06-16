using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Lookloop.ItemManager;

namespace Lookloop.ItemManager.Editor
{
/// <summary>
/// Editor utility that adds a right‑click menu item in the Project window
/// to quickly create an <see cref="ItemTable"/> asset.
/// Uses <c>ScriptableObject.CreateInstance</c> + <c>AssetDatabase.CreateAsset</c>
/// under the hood.
/// </summary>
public static class ItemTableEditorTools
{
    // Placed under Assets → Create; higher priority pushes it further down
    const string MenuPath = "Assets/Create/ItemTable";

#if UNITY_EDITOR
    [UnityEditor.MenuItem(MenuPath, priority = 81)]
    static void CreateItemTableAsset()
    {
        // Must use CreateInstance, not new ScriptableObject
        var asset = ScriptableObject.CreateInstance<ItemTable>();
        asset.ItemName = "New Item";
        asset.ItemDescription = string.Empty;

        // Generate a unique path so we never overwrite an existing file
        string folder = GetSelectedFolderOrAssets();
        string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/ItemTable.asset");

        AssetDatabase.CreateAsset(asset, path);
        // Support Ctrl+Z to undo the creation
        Undo.RegisterCreatedObjectUndo(asset, "Create ItemTable");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        // Flash the new asset in the Project window so the user can find it
        EditorGUIUtility.PingObject(asset);
    }

    /// <summary>If a folder is selected, use it; if a file is selected, use its parent;
    /// otherwise fall back to the Assets root.</summary>
    static string GetSelectedFolderOrAssets()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
            return "Assets";

        if (AssetDatabase.IsValidFolder(path))
            return path;

        string dir = Path.GetDirectoryName(path);
        return string.IsNullOrEmpty(dir) ? "Assets" : dir.Replace("\\", "/");
    }
#endif
}
}
