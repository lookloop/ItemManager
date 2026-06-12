namespace Lookloop.ItemManager
{
    /// <summary>
    /// detail 面板填充接口。
    /// </summary>
    public interface IDetailFiller
    {
        void Fill(Core core, Container container, int itemKey);
    }
}
