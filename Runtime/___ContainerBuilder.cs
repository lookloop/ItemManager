using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
/// <summary>
/// 容器 UI 构建 — 生成 Container→Mask→Grid→Cell 完整层级。
/// 不涉及数据，只管拼 UI。
/// </summary>
public static class ContainerBuilder
{
    /// <summary>遍历 mods 数组，逐项构建 + 注册容器</summary>
    public static void BuildAll(Core core)
    {
        foreach (var spec in core.specs)
        {
            ContainerMod containermod = new ContainerMod();
            if (spec.prefab != null)
                BuildFromPrefab(core, spec.prefab, containermod);
            else
                Build(core, spec);
        }
    }
    public static void BuildFromPrefab(Core core, RectTransform prefab, ContainerMod containermod)
    {
        var instance = Object.Instantiate(prefab, core.transform);
        {
            var allChildren = instance.GetComponentsInChildren<RectTransform>(true);
            var list = new System.Collections.Generic.List<RectTransform>();
            foreach (var tr in allChildren)
            {
                if (tr.CompareTag("Cell"))
                    list.Add(tr);
            }
            for (int i = 0; i < list.Count; i++)
                list[i].name = i.ToString();
            containermod.cells = list.ToArray();
            containermod.items = new Item[containermod.cells.Length];
            containermod.container = instance;
            ContainerManager.containers.Add(containermod);
        }
    }
    public static GameObject Build(Core core, ContainerSpec spec)
    {
        
    }
}
}