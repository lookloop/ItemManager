using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{

public static class MiscInit
{
    public static Image itemImage;
    public static Image edge;
    public static TextMeshProUGUI count;

    /// <summary>在 parent 下生成 ItemUI 三兄弟空对象，索引写入静态字段</summary>
    public static void CreateItemUI(Transform parent)
    {
        var itemUIGo = new GameObject("ItemUI", typeof(RectTransform), typeof(Image));
        itemUIGo.transform.SetParent(parent, false);
        itemImage = itemUIGo.GetComponent<Image>();

        var edgeGo = new GameObject("edge", typeof(RectTransform), typeof(Image));
        edgeGo.transform.SetParent(parent, false);
        edge = edgeGo.GetComponent<Image>();

        var countGo = new GameObject("count", typeof(RectTransform), typeof(TextMeshProUGUI));
        countGo.transform.SetParent(parent, false);
        count = countGo.GetComponent<TextMeshProUGUI>();
    }
}

}
