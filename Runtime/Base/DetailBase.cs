using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Detail 面板填充基类 — 挂在 detailRect prefab 上。
    /// 派生类 override Fill 做自定义详情渲染。
    /// </summary>
    public abstract class DetailBase : MonoBehaviour
    {
        public abstract Task Fill(Core core, Container container, int itemKey);
    }
}
