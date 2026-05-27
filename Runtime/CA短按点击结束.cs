using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class 短按点击结束
{
    public static void Execute(UIResponder _this, PointerEventData eventData)
    {
        Debug.Log("执行程序：短按普通点击结算 (Short Click)");
        // 1. 获取当前 eventData 对应的第一个物品（即当前点击的 UI 物体）
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        if (clickedObject != null)
        {
            // 使用 System.Array.IndexOf 获取对象在 cellRegistry 中的索引
            int index = System.Array.IndexOf(_this.cellRegistry, clickedObject);
            if (index >= 0 && index < _this.items.Length)
            {
                // 3. 得到 id
                Item item = _this.items[index];
                if (item != null)
                {
                    Debug.Log($"点击了索引: {index}, 物品 ID: {item.Id}");

                    // 4. 使用 id 调用 addressableLoader 获得 table 并显示详情
                    ShowItemDetail(_this, clickedObject, item.Id.ToString());
                }
                else
                {
                    Debug.Log($"点击了索引: {index}, 但该位置没有物品 (空格子)");
                }
                }
            }
    }

    // 辅助方法：根据 id 获取信息并展示
    public static async void ShowItemDetail(UIResponder _this, GameObject target, string id)
    {
        ItemTable table = await _this.GetItemTable(id);
        if (table != null)
        {
            SetPanelPosition(_this, target);
            SetPanelContent(_this, table);
            
            if (_this.Panel != null)
            {
                _this.Panel.gameObject.SetActive(true);
            }
        }
    }

    // 辅助方法：设置整个图像（面板）的位置
    

    // 辅助方法：给图像三个地方赋值（图标、名称、描述）
    private static void SetPanelContent(UIResponder _this, ItemTable table)
    {
        if (_this.IconImage != null)
        {
            // IconImage 本身作为储物格背景，保持不变
            
            // 1. 访问 IconImage 的唯一子对象（即物品图标）
            if (_this.IconImage.transform.childCount > 0)
            {
                Transform itemIconTransform = _this.IconImage.transform.GetChild(0);
                Image itemIconImage = itemIconTransform.GetComponent<Image>();
                if (itemIconImage != null)
                {
                    itemIconImage.sprite = table.ItemSprite;
                    itemIconImage.color = Color.white; // 确保颜色正常显示
                }

                // 2. 访问物品图标的唯一子对象（即光晕图标）
                if (itemIconTransform.childCount > 0)
                {
                    Transform glowTransform = itemIconTransform.GetChild(0);
                    Image glowImage = glowTransform.GetComponent<Image>();
                    if (glowImage != null)
                    {
                        glowImage.sprite = table.GlowSprite;
                        glowImage.color = Color.white; // 确保颜色正常显示
                    }
                }
            }
        }
        
        if (_this.NameText != null) _this.NameText.text = table.ItemName;
        if (_this.DescText != null) _this.DescText.text = table.ItemDescription;
    }

    private static void SetPanelPosition(UIResponder _this, GameObject target)
    {
        if (_this.Panel == null || target == null) return;

        RectTransform targetRT = target.transform as RectTransform;
        RectTransform canvasRT = _this.canvas.transform as RectTransform;

        // 1. 获取 cell 的中心点在自身局部坐标系下的位置
        Vector2 cellCenterLocal = targetRT.rect.center;
        
        // 2. 将中心点转换为 Canvas 的局部坐标
        Vector2 canvasCenterPos = UI坐标转换.局部转目标局部(targetRT, canvasRT, cellCenterLocal);

        Vector2 finalPosition;

        // 3. 判断中心点在 Canvas 的左边还是右边 (x >= 0 表示在右边)
        if (canvasCenterPos.x >= 0)
        {
            // 在右边，详情面板显示在左边
            // 获取 cell 左边缘的局部坐标
            Vector2 cellLeftLocal = new Vector2(targetRT.rect.xMin, targetRT.rect.center.y);
            // 转换为 Canvas 坐标
            finalPosition = UI坐标转换.局部转目标局部(targetRT, canvasRT, cellLeftLocal);

            // 设置详情面板的锚点和轴心
            // 保持 anchor 为 0.5, 0.5，这样可以直接使用 finalPosition 作为 anchoredPosition
            _this.Panel.anchorMin = new Vector2(0.5f, 0.5f);
            _this.Panel.anchorMax = new Vector2(0.5f, 0.5f);
            _this.Panel.pivot = new Vector2(1f, 0.5f); // 轴心 x=1, y=0.5
        }
        else
        {
            // 在左边，详情面板显示在右边
            // 获取 cell 右边缘的局部坐标
            Vector2 cellRightLocal = new Vector2(targetRT.rect.xMax, targetRT.rect.center.y);
            // 转换为 Canvas 坐标
            finalPosition = UI坐标转换.局部转目标局部(targetRT, canvasRT, cellRightLocal);

            // 设置详情面板的锚点和轴心
            _this.Panel.anchorMin = new Vector2(0.5f, 0.5f);
            _this.Panel.anchorMax = new Vector2(0.5f, 0.5f);
            _this.Panel.pivot = new Vector2(0f, 0.5f); // 轴心 x=0, y=0.5
        }

        // --- 处理 Y 轴越界问题 ---
        // 获取详情面板的高度一半
        float panelHalfHeight = _this.Panel.rect.height / 2f;
        // 获取 Canvas 的 Y 轴极限值（因为 anchor 是 0.5, 0.5，所以极限值就是 Canvas 高度的一半）
        float canvasMaxY = canvasRT.rect.height / 2f;
        float canvasMinY = -canvasMaxY;

        // 2毫米的物理尺寸，因为 Canvas Scaler 设置了 Physical Unit 为 Millimeters
        float padding = 2f; 

        // 检查上半部分是否越界
        if (finalPosition.y > 0)
        {
            // 如果 当前Y位置 到 顶部极限 的距离 小于 面板高度的一半
            if (canvasMaxY - finalPosition.y < panelHalfHeight)
            {
                // 限制在顶部极限 - 面板高度一半 - 边距
                finalPosition.y = canvasMaxY - panelHalfHeight - padding;
            }
        }
        // 检查下半部分是否越界
        else
        {
            // 如果 当前Y位置 到 底部极限 的距离 小于 面板高度的一半（注意这里都是负数，用绝对值或者相减判断）
            if (finalPosition.y - canvasMinY < panelHalfHeight)
            {
                // 限制在底部极限 + 面板高度一半 + 边距
                finalPosition.y = canvasMinY + panelHalfHeight + padding;
            }
        }

        // 直接赋值位置
        _this.Panel.anchoredPosition = finalPosition;
    }
}
