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
            // sourceRect 就是被按下的 Cell 的 RectTransform，name 就是 key
            if (!int.TryParse(core.sourceRect.name, out int key)) return;

            var container = core.sourceContainer;
            if (container == null || container.items == null) return;

            var item = container.items[key];
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

            // 赋值图标、边框、数量
            OtherTool.dragItem.sprite = table.ItemSprite;
            OtherTool.dragEdge.sprite = table.GlowSprite;
            OtherTool.dragCount.text = item.Count.ToString();

            // 显示
            OtherTool.dragRect.gameObject.SetActive(true);
        }
    }
}
