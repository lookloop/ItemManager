using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// TouchItem — 处理长按 Cell 后物品拖拽的交互逻辑。
    /// </summary>
    public static class TouchItem
    {

        public static async Task ExtractItem(Core core)
        {
            var container = core.sourceContainer;
            if (container == null || container.items == null) return;
            int itemKey = core.sourceItemKey;
            var item = container.items[itemKey];
            if (item == null || item.Id == 0) return;

            // 屏幕坐标 → Canvas 本地坐标，移动 dragRect
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.canvas.transform as RectTransform,
                core.eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);

            OtherTool.dragRect.anchoredPosition = localPos;
            // 通过 id 异步加载 ItemTable
            var table = await core.GetItemTable(item.Id.ToString());
            if (table == null) return;
            OtherTool.dragItem.sprite = table.ItemSprite;
            OtherTool.dragEdge.sprite = table.GlowSprite;
            OtherTool.dragCount.text = item.Count.ToString();
            OtherTool.dragRect.gameObject.SetActive(true);
            SetItem.NoView(core.sourceContainer, core.sourceItemKey);
        }

        public static void OnDrag(Core core)
        {
            if (!OtherTool.dragRect.gameObject.activeSelf) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.canvas.transform as RectTransform,
                core.eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);
            OtherTool.dragRect.anchoredPosition = localPos;

            // 射线检测当前拖拽下方碰到的对象
            var raycast = core.eventData.pointerCurrentRaycast;
            if (raycast.gameObject != null)
            {
                core.targetRect = raycast.gameObject.GetComponent<RectTransform>();

                // 沿父级向上找 tag=Container，通过 name 定位 container
                core.targetContainer = null;
                var t = core.targetRect != null ? core.targetRect.parent : null;
                while (t != null)
                {
                    if (t.CompareTag("Container"))
                    {
                        if (int.TryParse(t.name, out int index) && index < core.containers.Length)
                            core.targetContainer = core.containers[index];
                        break;
                    }
                    t = t.parent;
                }

                if (core.targetContainer != null && core.targetContainer.containerRect != null)
                    core.targetContainer.containerRect.SetAsLastSibling();

                // 命中 Cell：Shadow 挂到目标 Cell 下，居中覆盖，同时计算 targetItemKey
                if (core.targetRect != null && core.targetRect.CompareTag("Cell"))
                {
                    if (core.targetContainer != null &&
                        int.TryParse(core.targetRect.name, out int cellKey))
                    {
                        core.targetItemKey = core.targetContainer.cells.Length *
                            (core.targetContainer.currentPage - 1) + cellKey;
                    }

                    OtherTool.Shadow.SetParent(core.targetRect, false);
                    OtherTool.Shadow.gameObject.SetActive(true);
                }
                else
                {
                    core.targetItemKey = null;
                    OtherTool.Shadow.SetParent(core.canvas.transform, false);
                    OtherTool.Shadow.gameObject.SetActive(false);
                }
            }
            else
            {
                core.targetRect = null;
                core.targetContainer = null;
                core.targetItemKey = null;
                OtherTool.Shadow.gameObject.SetActive(false);
            }
        }
    }
}
