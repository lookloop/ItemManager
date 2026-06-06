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
            Debug.Log($"[CellTouch] 长按有效 — holdTime:{core.holdTime:F2}");
        }
    }
}
