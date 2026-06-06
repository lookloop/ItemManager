using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Lookloop.ItemManager
{

public static class CellTouch
    {
        public static bool isLongPress;
        public static PointerEventData downEvent;

        public static void BeginTouch(Core core, PointerEventData eventData)
        {
            isLongPress = false;
            downEvent = eventData;
            Debug.Log($"[CellTouch] 按下 — cell:{core.hitRect?.name}");
        }

        public static void LongPress(Core core)
        {
            if (core.isDrag) return;
            isLongPress = true;

            // 1. 定位 MiscInit.parent 到指针 canvas 坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.canvas.transform as RectTransform,
                downEvent.position,
                core.canvas.worldCamera,
                out Vector2 localPos);
            MiscInit.parent.anchoredPosition = localPos;

            // 2. 从 hitRect 名字提取 cellIndex（"Cell3"→3, "5"→5）
            string name = core.hitRect.name;
            if (name.StartsWith("Cell")) name = name.Substring(4);
            int cellIndex = int.Parse(name);

            // 3. 查找所属 ContainerMod
            ContainerMod mod = null;
            foreach (var m in ContainerManager.containers)
            {
                if (core.hitRect.IsChildOf(m.container))
                    { mod = m; break; }
            }
            if (mod == null) return;

            // 4. 计算全局 itemKey
            int itemKey = (mod.currentPage - 1) * mod.cells.Length + cellIndex;

            // 5. 赋值显示
            MiscInit.AssignItemData(core, mod, itemKey);

            Debug.Log($"[CellTouch] 长按有效 — cell:{cellIndex} itemKey:{itemKey} pos:{localPos}");
        }
    }
}
