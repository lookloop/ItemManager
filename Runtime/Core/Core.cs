using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public partial class Core : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// 空白区域点击 → 关闭所有 Detail 面板。
    /// Cell/Container/TurnPage 的交互已由各自的 Handler 组件处理，
    /// Core 不再参与路由分发。
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        var clicked = eventData.pointerCurrentRaycast.gameObject;

        // 点击了 Core 自身（全屏透明接收器）= 空白区域
        // 或者点击了 Container 本身（非 Cell/按钮区域）
        if (containers != null)
        {
            foreach (var c in containers)
            {
                var df = c?.detailFiller as DetailFiller;
                if (df != null && df.gameObject.activeSelf &&
                    (clicked == null || !clicked.transform.IsChildOf(df.transform)))
                {
                    df.gameObject.SetActive(false);
                }
            }
        }

        // 将命中的 container 提到最前
        if (clicked != null)
        {
            Transform t = clicked.transform;
            while (t != null)
            {
                if (t.CompareTag("Container"))
                {
                    t.SetAsLastSibling();
                    break;
                }
                t = t.parent;
            }
        }
    }
}
}
