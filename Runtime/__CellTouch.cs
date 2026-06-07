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
        public static RectTransform grid;
        public static RectTransform mask;
        public static Vector2 gridStartPos;
        public static Vector2 startLocal;
        public static int sourceItemKey;
        public static ContainerMod sourceMod;

        public static void BeginTouch(Core core, PointerEventData eventData)
        {
            isLongPress = false;
            downEvent = eventData;
            grid = core.hitRect?.parent as RectTransform;
            mask = grid?.parent as RectTransform;
            gridStartPos = grid.anchoredPosition;

            if (mask != null)
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    mask, eventData.position, core.canvas.worldCamera, out startLocal);
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
            sourceItemKey = itemKey;
            sourceMod = core.hitContainerMod;
            // 4. 赋值显示 + 隐藏源头 Cell
            MiscInit.AssignItemData(core, itemKey);
            ItemsController.HideItemUI(sourceMod, itemKey);

        }

        public static void OnDrag(Core core, PointerEventData eventData)
        {
            if (!isLongPress)
            {
                CellTouch_Grid.OnDrag(core, eventData);
                return;
            }
            CellTouch_Drag.OnDrag(core, eventData);
        }

        public static void EndTouch(Core core)
        {
            if (isLongPress && CellTouch_Drag.lastEventData != null)
            {
                var targetGo = CellTouch_Drag.lastEventData.pointerCurrentRaycast.gameObject;
                if (targetGo != null && targetGo.CompareTag("Cell"))
                {
                    // 找到目标所属容器
                    ContainerMod targetMod = null;
                    foreach (var m in ContainerManager.containers)
                    {
                        if (targetGo.transform.IsChildOf(m.container))
                        {
                            targetMod = m;
                            break;
                        }
                    }

                    if (targetMod != null)
                    {
                        int targetCellIndex = int.Parse(targetGo.name);
                        int targetItemKey = (targetMod.currentPage - 1) * targetMod.cells.Length + targetCellIndex;

                        if (targetMod == sourceMod)
                        {
                            // 同容器内交换
                            if (targetItemKey != sourceItemKey)
                                ItemsController.SwapItem(core, sourceMod, sourceItemKey, targetItemKey);
                        }
                        else
                        {
                            // 跨容器交换
                            var srcItem = sourceMod.items[sourceItemKey];
                            var dstItem = targetMod.items[targetItemKey];
                            sourceMod.items[sourceItemKey] = dstItem;
                            targetMod.items[targetItemKey] = srcItem;
                            ItemsController.SetViewItem(core, sourceMod, sourceItemKey);
                            ItemsController.SetViewItem(core, targetMod, targetItemKey);
                        }
                    }
                }
            }
            else if (!isLongPress && !core.isDrag && core.hitContainerMod != null && core.hitContainerMod.detail != null)
            {
                // 短按 Cell → 显示 detail 面板 + 填充数据
                core.ShowDetail(core.hitContainerMod);
            }
            Reset();
        }

        public static void Reset()
        {
            MiscInit.HideAll();
            isLongPress = false;
            downEvent = null;
            grid = null;
            mask = null;
            sourceMod = null;
        }
    }
}
