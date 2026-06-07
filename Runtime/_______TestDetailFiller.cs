using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 测试用 detail 填充脚本。
    /// 挂到 detail 预制体根节点上，根节点需有 Image + 子级 TMP。
    /// Fill() 从 ItemTable Addressables 异步加载物品描述并显示。
    /// </summary>
    public class TestDetailFiller : MonoBehaviour, IDetailFiller
    {
        [Tooltip("用于显示物品描述的 TextMeshProUGUI")]
        public TextMeshProUGUI descText;

        Image _bgImage;

        void Awake()
        {
            _bgImage = GetComponent<Image>();
            if (_bgImage == null)
                _bgImage = gameObject.AddComponent<Image>();

            // 初始隐藏
            gameObject.SetActive(false);
        }

        public void Fill(ContainerMod mod, int itemKey)
        {
            if (itemKey < 0 || itemKey >= mod.items.Length)
            {
                Debug.LogWarning($"[TestDetailFiller] itemKey {itemKey} 越界");
                return;
            }

            var item = mod.items[itemKey];
            if (item == null || item.Id == 0)
            {
                descText.text = "空";
                return;
            }

            // 先显示基本信息
            descText.text = $"Id: {item.Id}\nType: {item.Type}\nTier: {item.Tier}\nCount: {item.Count}\n加载描述中...";

            // 异步加载 ItemTable 拿描述
            _ = LoadDescriptionAsync(item.Id.ToString());
        }

        async Task LoadDescriptionAsync(string key)
        {
            // 找场景中的 Core 拿缓存
            var core = Object.FindAnyObjectByType<Core>();
            if (core == null)
            {
                descText.text += "\n(Core 未找到)";
                return;
            }

            var table = await core.GetItemTable(key);
            if (table != null && !string.IsNullOrEmpty(table.ItemDescription))
            {
                descText.text = $"{table.ItemName}\n\n{table.ItemDescription}";
            }
            else
            {
                descText.text += $"\n(ItemTable 加载失败或描述为空: {key})";
            }
        }
    }
}
