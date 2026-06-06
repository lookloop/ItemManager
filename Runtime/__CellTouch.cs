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
            // 2. 从 hitRect 名字提取 cellIndex（名字即数字 "0","1","2"...）
            int cellIndex = int.Parse(core.hitRect.name);
            // 4. 计算全局 itemKey
            int itemKey = (core.hitContainerMod.currentPage - 1) * core.hitContainerMod.cells.Length + cellIndex;
            // 5. 赋值显示
            MiscInit.AssignItemData(core, itemKey);

        }
    }
}
