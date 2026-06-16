using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
    public partial class CellTouch
    {
        // ═══════════════════════════════════════════════
        //  纯点击 → 显示详情面板
        // ═══════════════════════════════════════════════
        void ShowDetail()
        {
            int globalKey = container.cells.Length * (container.currentPage - 1) + cellKey;
            core.Launch(container.detailFiller?.Fill(core, container, globalKey) ?? Task.CompletedTask);
        }
    }
}
