using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// TouchItem — 处理长按 Cell 后物品拖拽的交互逻辑。
    /// </summary>
    public static class TouchItem
    {

        public static async void ExtractItem(Core core)
        {
            // sourceRect 就是被按下的 Cell 的 RectTransform，name 就是本页索引
            if (!int.TryParse(core.sourceRect.name, out int cellKey)) return;
            var container = core.sourceContainer;
            if (container == null || container.items == null) return;
            // cell 索引 → 全局 items 索引
            int itemKey = container.cells.Length * (container.currentPage - 1) + cellKey;
            var item = container.items[itemKey];
            if (item == null) return;

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

                // 命中 Cell：Shadow 挂到目标 Cell 下，居中覆盖
                if (core.targetRect != null && core.targetRect.CompareTag("Cell"))
                {
                    OtherTool.Shadow.SetParent(core.targetRect, false);
                    OtherTool.Shadow.gameObject.SetActive(true);
                }
                else
                {
                    OtherTool.Shadow.SetParent(core.canvas, false);
                    OtherTool.Shadow.gameObject.SetActive(false);
                }
            }
            else
            {
                core.targetRect = null;
                core.targetContainer = null;
                OtherTool.Shadow.gameObject.SetActive(false);
            }
        }
    }
}
