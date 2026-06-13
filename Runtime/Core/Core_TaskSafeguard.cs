using System;
using System.Threading.Tasks;

namespace Lookloop.ItemManager
{
public partial class Core
{
    /// <summary>
    /// Fire-and-forget with screen debug output and optional failure callback.
    /// </summary>
    public async void FireAndForget(Task task, Action<Exception> onError = null)
    {
        try { await task; }
        catch (Exception e)
        {
            if (tmpText != null)
                tmpText.text = $"[Task Error] {e.GetType().Name}: {e.Message}";
            onError?.Invoke(e);
        }
    }
}
}
