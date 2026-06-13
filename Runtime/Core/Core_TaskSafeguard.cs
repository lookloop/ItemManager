using System;
using System.Threading.Tasks;

namespace Lookloop.ItemManager
{
public partial class Core
{
    public async void FireAndForget(Task task)
    {
        try { await task; }
        catch (Exception e)
        {
            if (tmpText != null)
                tmpText.text = $"[Task Error] {e.GetType().Name}: {e.Message}";
        }
    }
}
}
