using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Detail 面板填充基类 — 挂载到 detailRect 预制体上。
    /// 继承此 MonoBehaviour 并实现 Fill 方法，即可响应物品点击、填充详情 UI。
    ///
    /// 用法：
    /// 1. 创建一个预制体，挂载继承此类的脚本
    /// 2. 拖入 ContainerSpec 的 detailRect 字段
    /// 3. Core 会在运行时自动调用 Fill(container, itemKey)
    /// </summary>
    public abstract class DetailFiller : MonoBehaviour, IDetailFiller
    {
        /// <summary>
        /// 当容器中有物品被选中时回调。
        /// </summary>
        /// <param name="container">当前容器实例</param>
        /// <param name="itemKey">选中物品的全局下标</param>
        public abstract void Fill(Container container, int itemKey);
    }
}
