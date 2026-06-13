# Item Manager

**纯代码驱动的多容器物品管理框架** — 背包、仓库、装备槽、商店… 一切格子系统的基础设施。

零预制体依赖。Addressables 异步加载。多容器并存。跨容器拖拽交换。

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black)](package.json)

---

## 架构

```
Core.cs                     ← 空白点击关闭 Detail / Container 置顶
├── Core_Fields.cs          ← 公开字段 (specs[] / font / shadowColor / pressTime …)
├── Core_Init.cs            ← 启动流程 (Awake + Start + 过期回收协程)
├── Core_Addressables.cs    ← ItemTable 异步加载 + 缓存 + 30min 过期回收
├── Core_ContainerBuilder.cs← Container → Mask → Grid → Cell 全程序化构建 + Handler 挂载
├── Core_SetItem.cs         ← 物品数据写入 + View 刷新 + NoView 隐藏
├── Core_SetPage.cs         ← 翻页逻辑 + 最后一页高度适配 + 滑动动画反馈
├── Core_DragTool.cs        ← 拖拽幽灵图标 + 悬停阴影构建
├── Core_RectUtility.cs     ← 程序化 RectTransform 创建（两重载）
└── Core_TaskSafeguard.cs   ← FireAndForget 异常捕获

数据层
├── Item.cs                 ← 物品运行时数据 (readonly struct: Id / Type / Tier / Count / Data)
├── ItemTable.cs            ← ScriptableObject 物品表 (图标 / 边框 / 名称 / 描述)
├── Cell.cs                 ← 格子 UI 引用 (cell / item 图标 / edge 边框 / count 数量)
├── Container.cs            ← 运行时容器实例 (Rect 引用 + items[] + cells[] + 翻页状态)
├── ContainerSpec.cs        ← 容器蓝图 (尺寸 / 每页格子 / 行数 / 视觉精灵 / 过滤器)
└── SetItemBase.cs          ← 容器准入过滤器基类 [SerializeReference]

基类
├── TouchBase.cs            ← 交互基类 (IPointerDown/Drag/UpHandler)，CellTouch / ContainerTouch / TurnPageTouch 继承它
└── DetailBase.cs           ← 详情面板基类，派生类 override Fill 做自定义详情渲染

触控层
├── CellTouch.cs            ← 格子交互主状态机 (PointerDown → Drag → Up 路由)
├── CellTouch_GridScroll.cs ← 短按拖拽滚动 Grid
├── CellTouch_ItemDrag.cs   ← 长按提取物品 + 幽灵拖拽 + 射线追踪目标 Cell
├── CellTouch_LongPressLoop.cs ← 长按计时协程 + 边缘滚动 + 边缘翻页每帧检测
├── CellTouch_Exchange.cs   ← 交换物品（双向准入检查）+ 状态重置
├── CellTouch_Detail.cs     ← 纯点击显示详情面板
├── ContainerTouch.cs       ← 容器整体拖拽
└── TurnPageTouch.cs        ← 翻页按钮点击

详情层
└── DetailFiller.cs         ← DetailBase 默认实现，异步加载 ItemTable + 自动定位

过滤器
├── TypeRestrictFilter.cs   ← 类型限制过滤器（只允许指定 Type 的物品进入）
└── TestFilter.cs           ← 示例过滤器（OddIdOnlyFilter：只允许奇数 Id）

测试
└── Test.cs                 ← 启动时随机填充物品 (EDITOR only, 1/3 概率)

Editor
├── ContainerSpecDrawer.cs  ← ContainerSpec 自定义折叠 Inspector + [SerializeReference] 类型下拉
└── ItemDataEditorTools.cs  ← Project 右键 → Create → ItemTable
```

---

## 特性

- **零预制体** — 所有 UI 通过 `new GameObject` 程序化生成，Container → Mask → Grid → Cell 完整层级
- **多容器并存** — 一个 `Core` 组件，`specs[]` 数组驱动任意数量独立容器面板
- **跨容器交换** — 长按拖拽物品可在不同容器之间互换位置，支持双向准入过滤器
- **Addressables 异步** — `ItemTable` 按需加载，30 分钟缓存过期自动回收
- **长按 + 边缘滚动** — 长按提取物品后拖到 Mask 边缘自动滚动列表，拖到左右边缘自动翻页
- **自定义 Inspector** — `ContainerSpec` 可折叠参数面板，新增元素自动填充默认值；`[SerializeReference]` 类型下拉选择过滤器
- **两构建模式** — 纯数据驱动（`Build`）或预制体实例化（`BuildPrefab`），通过 `prefabRect` 是否为空切换
- **详情接口** — 继承 `DetailBase` 并 override `Fill(Core, Container, int)`，挂载到 `detailRect` 预制体即可
- **准入过滤器** — `SetItemBase.CanExchange(incoming, outgoing)` 在交换时双向检查，支持装备槽、消耗品槽等受限容器

---

## 安装

### Unity Package Manager (Git URL)

```
https://github.com/lookloop/ItemManager.git
```

### 依赖

| 包 | 最低版本 |
|---|---|
| `com.unity.addressables` | 1.21.0 |
| `com.unity.textmeshpro` | 3.0.0 |
| `com.unity.ugui` | 1.0.0 |

---

## 快速开始

### 数据驱动模式（无预制体）

1. Canvas 下新建空 GameObject，挂载 `Core` 组件
2. 设置 `Font`（TMP 字体资源）
3. 在 `Specs` 数组中填入容器参数：

```
specs[0]:
    Total Items        = 80        // 总物品容量
    Every Page Cells   = 40        // 每页格子数
    Row                = 5         // 每行格子数
    Cell Width         = 10        // 格子边长
    Mask Height        = 40        // 可视区域高度
    Container Fill Up  = 8         // 容器上边距
    Container Fill Down = 4        // 容器下边距
```

4. 运行 → 自动生成完整容器 UI，随机填充测试物品

### 预制体模式

将 `Prefab Rect` 拖入 `ContainerSpec`：
- 自动 `Instantiate` 预制体
- 扫描所有 `tag="Cell"` 的子对象作为格子
- `items` 数组长度 = Cell 数量，单页模式（无翻页）

### 创建物品表

Project 窗口右键 → `Create → ItemTable`，填入：
- `Id` — 物品唯一标识
- `Item Sprite` — 物品图标
- `Glow Sprite` — 边框/光效
- `Item Name` / `Item Description` — 名称与描述

将 `ItemTable` 标记为 Addressable，Key 使用 `Id.ToString()`。

---

## 触控交互

| 操作 | 行为 |
|---|---|
| 点击 Cell（无拖拽） | 显示详情面板（调用 `DetailBase.Fill`） |
| 点击 Cell 后拖拽（未长按） | Grid 拖拽滚动 |
| 长按 Cell (0.3s) | 提取物品到幽灵拖拽，原 Cell 隐藏 |
| 拖拽中命中 Cell | 悬停阴影显示，实时追踪目标 Cell |
| 拖拽到 Mask 上下边缘 | 自动滚动 Grid |
| 拖拽到 Mask 左右边缘 | 自动翻页（带冷却时间） |
| 松开手指（长按+拖拽） | 交换 source ↔ target 物品，UI 自动刷新 |
| 点击翻页按钮 | 上一页 / 下一页 |
| 点击页码输入框 | 输入数字跳页 |
| 点击空白区域 | 关闭所有 Detail 面板 |

---

## API

### Core（公开字段）

| 字段 | 类型 | 说明 |
|---|---|---|
| `specs` | `ContainerSpec[]` | 容器蓝图数组，每项生成一个独立容器 |
| `font` | `TMP_FontAsset` | 全局 TMP 字体 |
| `fontSize` | `float` | 全局字号（默认 3.9） |
| `pressTime` | `float` | 长按判定时间（默认 0.3s） |
| `scrollSpeed` | `float` | 边缘滚动速度（默认 60） |
| `edgeThreshold` | `float` | 边缘触发距离（默认 3） |
| `turnThreshold` | `float` | 翻页冷却时间（默认 0.5s） |
| `flipDuration` | `float` | 翻页动画时长（默认 0.5s） |
| `shadowColor` | `Color` | 拖拽悬停阴影颜色（默认黑色半透明） |

### Core.SetItem — 物品数据写入

```csharp
// 创建物品并自动刷新 UI（只有当前页才会刷新视图）
core.SetItem(container, itemKey, id, type, tier, count, data);

// 或者直接传入 Item struct
core.SetItem(container, itemKey, new Item(id, type, tier, count, data));
```

### Core.View / Core.NoView — 视图刷新

```csharp
// 异步加载 ItemTable 并刷新单个 Cell 的图标/边框/数量
await core.View(container, itemKey);

// 隐藏 Cell 显示（不修改数据）
core.NoView(container, itemKey);
```

### Core.SetPage — 翻页

```csharp
// 跳转到指定页，刷新所有可见 Cell
core.SetPage(container, page);
```

### Core.GetItemTable — 加载物品表

```csharp
// 异步加载 ItemTable（带缓存）
var table = await core.GetItemTable(itemId.ToString());
```

### SetItemBase — 准入过滤器

```csharp
[Serializable]
public class SetItemBase
{
    // 交换前准入检查（双向调用）
    public virtual bool CanExchange(Item incoming, Item outgoing) => true;

    // SetItem 完成后的回调
    public virtual void OnItemSet(Container container, int itemKey) { }
}
```

内置实现：`TypeRestrictFilter`（限制 Type）、`OddIdOnlyFilter`（测试用，限制奇数 Id）。

### DetailBase — 详情面板

```csharp
public abstract class DetailBase : MonoBehaviour
{
    public abstract Task Fill(Core core, Container container, int itemKey);
}
```

将继承 `DetailBase` 的组件挂在 `detailRect` 预制体上，点击物品时自动回调。默认实现：`DetailFiller`。

---

## 数据流

```
按下 Cell
  → OnPointerDown: 启动长按协程 LongPressTimer (pressTime 倒计时)
    同时记录 Grid 起始位置用于短按滚动
  → OnDrag:
      - 长按已触发 → DragItem (幽灵跟随手指 + 射线追踪目标 Cell + 阴影定位)
      - 长按未触发 → 取消计时 → ScrollGrid (Grid 跟随手指滚动)
  → LongPressTimer 每帧检测:
      - 手指在 Mask 上下边缘 → 自动滚动 Grid
      - 手指在 Mask 左右边缘 → 自动翻页（带冷却）
  → OnPointerUp:
      - 长按+拖拽 → Exchange
          → 计算 srcKey / tgtKey
          → 读取 srcItem / tgtItem
          → 双向 CanExchange 检查（src 容器过滤器 + tgt 容器过滤器）
          → core.SetItem 互换数据
      - 无长按且无拖拽 → ShowDetail (调用 DetailBase.Fill)
      - 统一 Reset:
          → 恢复 source Cell 显示（如果在当前页）
          → 隐藏拖拽幽灵和阴影
          → 清空所有状态
```

---

## License

MIT © 2026 覃健培 (Qin Jianpei)
