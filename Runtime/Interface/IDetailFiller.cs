using System.Threading.Tasks;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// detail 面板填充接口。
    /// </summary>
    public interface IDetailFiller
    {
        Task Fill(Core core, Container container, int itemKey);
    }
}
