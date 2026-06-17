using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
    public partial class CellTouch
    {
        // ── Tap (no drag) → show the detail panel ──
        void ShowDetail()
        {
            int globalKey = container.cells.Length * (container.currentPage - 1) + cellKey;
            core.Launch(container.detailFiller?.Fill(container, globalKey) ?? Task.CompletedTask);
        }
    }
}
