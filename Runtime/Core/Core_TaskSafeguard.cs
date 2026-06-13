using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    /// <summary>
    /// Safeguard for discarded Tasks — wraps fire-and-forget so exceptions
    /// are logged to the Console instead of being silently swallowed.
    /// </summary>
    public async void FireAndForget(Task task)
    {
        try { await task; }
        catch (System.Exception e)
        {
            Debug.LogException(e);

            if (tmpText != null)
                tmpText.text = $"[Task Error] {e.GetType().Name}: {e.Message}";
        }
    }
}
}
