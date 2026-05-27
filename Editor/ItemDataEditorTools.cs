using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// 编辑器扩展：在 Project 窗口用菜单批量/快速创建 <see cref="ItemTable"/> 资源。
/// 与资源上右键 Create → ItemTable 相同，均为 CreateInstance + CreateAsset 写入磁盘。
/// </summary>
public static class ItemTableEditorTools
{
    // 挂在 Unity 顶部菜单 Assets → Create 下；priority 越大越靠下（与内置 Create 项错开）
    const string MenuPath = "Assets/Create/ItemTable（脚本创建）";

#if UNITY_EDITOR
    [UnityEditor.MenuItem(MenuPath, priority = 81)]
    static void CreateItemTableAsset()
    {
        // 必须用 CreateInstance，勿 new ScriptableObject
        var asset = ScriptableObject.CreateInstance<ItemTable>();
        asset.ItemName = "新物品";
        asset.ItemDescription = string.Empty;

        // 生成唯一路径，避免覆盖已有 ItemTable.asset
        string folder = GetSelectedFolderOrAssets();
        string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/ItemTable.asset");

        AssetDatabase.CreateAsset(asset, path);
        // 支持 Ctrl+Z 撤销本次创建
        Undo.RegisterCreatedObjectUndo(asset, "Create ItemTable");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        // 在 Project 中闪一下，便于看到新文件位置
        EditorGUIUtility.PingObject(asset);
    }

    /// <summary>当前选中为文件夹则用该文件夹；选中资源则用其所在目录；无选中则用 Assets 根。</summary>
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
