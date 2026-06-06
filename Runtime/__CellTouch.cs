using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Lookloop.ItemManager
{

public static class CellTouch
    {
        public static void LongPress(Core core)
        {
            Debug.Log($"[CellTouch] 长按触发 — tag:{core.atTag}  holdTime:{core.holdTime:F2}");
        }
    }
}
