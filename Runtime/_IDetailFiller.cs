namespace Lookloop.ItemManager
{
    /// <summary>
    /// detail 面板填充接口。
    /// 挂载在 detail 预制体根节点上，ContainerBuilder 构建时会缓存此引用到 ContainerMod.detailFiller。
    /// CellTouch 短按 Cell 时调用 Fill() 灌入数据。
    /// </summary>
    public interface IDetailFiller
    {
        void Fill(ContainerMod mod, int itemKey);
    }
}
