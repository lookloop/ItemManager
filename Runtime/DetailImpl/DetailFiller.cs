using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Detail 面板 — 挂载到 detailRect 预制体上，实现 IDetailFiller 接口。
    /// Core 会在物品被选中时自动调用 Fill(container, itemKey)。
    ///
    /// 用法：
    /// 1. 在这里添加 UI 字段（Image、TMP_Text 等）
    /// 2. 在 Fill 方法中编写填充逻辑
    /// 3. 将预制体拖入 ContainerSpec 的 detailRect 字段
    /// </summary>
    public class DetailFiller : MonoBehaviour, IDetailFiller
    {
        // ── 在这里添加你的 UI 字段 ──



        // ── 填充回调 ──
        public void Fill(Container container, int itemKey)
        {
            // 在这里写填充逻辑
        }
    }
}
