namespace Lookloop.ItemManager
{
    /// <summary>
    /// detail 面板填充接口。
    /// </summary>
    public interface IDetailFiller
    {
        void Fill(Container container, int itemKey);
    }
}
