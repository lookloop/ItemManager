using System;
using System.Threading.Tasks;

namespace Lookloop.ItemManager
{
public partial class Core
{
    public async void Launch(Task task)
    {
        try { await task; }
        catch (Exception e)
        {
            ShowTip($"[Task Error] {e.GetType().Name}: {e.Message}");
        }
    }
}
}
