using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    public partial class CellTouch
    {
        // ═══════════════════════════════════════════════
        //  提取物品 → 幽灵图标
        // ═══════════════════════════════════════════════
        async Task ExtractItem(PointerEventData eventData)
        {
            if (container == null || container.items == null) return;
            int globalKey = container.cells.Length * (container.currentPage - 1) + cellKey;
            var item = container.items[globalKey];
            if (item.Id == 0) return;

            core.dragSourceContainer = container;
            core.dragSourceItemKey = globalKey;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.canvas.transform as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);

            core.dragRect.anchoredPosition = localPos;

            var table = await core.GetItemTable(item.Id.ToString());
            if (table == null) return;

            core.dragItem.sprite = table.ItemSprite;
            core.dragEdge.sprite = table.GlowSprite;
            core.dragCount.text = item.Count.ToString();
            core.dragRect.gameObject.SetActive(true);

            core.NoView(container, globalKey);
        }

        // ═══════════════════════════════════════════════
        //  拖拽幽灵 + 射线找目标
        // ═══════════════════════════════════════════════
        void DragItem(PointerEventData eventData)
        {
            if (!core.dragRect.gameObject.activeSelf) return;

            // 幽灵跟随手指
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.canvas.transform as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);
            core.dragRect.anchoredPosition = localPos;

            // 射线找目标
            var raycast = eventData.pointerCurrentRaycast;
            if (raycast.gameObject != null)
            {
                var other = raycast.gameObject.GetComponent<CellTouch>();
                if (other != null && other != this)
                {
                    targetCell = other;

                    if (targetCell.container != null && targetCell.container.containerRect != null)
                        targetCell.container.containerRect.SetAsLastSibling();

                    var cellRect = targetCell.container.cells[targetCell.cellKey].cell;
                    core.Shadow.SetParent(cellRect, false);
                    core.Shadow.gameObject.SetActive(true);
                }
                else
                {
                    ClearTarget();
                }
            }
            else
            {
                ClearTarget();
            }
        }

        void ClearTarget()
        {
            targetCell = null;
            core.Shadow.SetParent(core.canvas.transform, false);
            core.Shadow.gameObject.SetActive(false);
        }
    }
}
