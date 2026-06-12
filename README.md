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
└── Core_Addressables.cs    ← ItemTable 异步加载 + 缓存 + 30min 过期回收

数据层
├── Item.cs                 ← 物品运行时数据 (readonly struct: Id / Type / Tier / Count)
├── ItemTable.cs            ← ScriptableObject 物品表 (图标 / 边框 / 名称 / 描述)
├── CellData.cs             ← 格子 UI 引用 (cell / item 图标 / edge 边框 / count 数量)
├── ContainerData.cs        ← 运行时容器实例 (Rect 引用 + items[] + cells[] + 翻页状态)
├── ContainerSpec.cs        ← 容器蓝图 (尺寸 / 每页格子 / 行数 / 视觉精灵)
└── IDetailFiller.cs        ← 详情面板填充接口

构建层
└── ContainerBuilder.cs     ← Container → Mask → Grid → Cell 全程序化构建 + Handler 挂载

基类
└── TouchBase.cs            ← 交互基类 (IPointerDown/Drag/UpHandler)，Cell/Container/TurnPage 继承它

触控层
├── CellHandler.cs          ← 格子全交互：短按滚动 Grid / 长按提取物品 / 幽灵拖拽 / 射线追踪 / 边缘滚动翻页 / 交换物品
├── ContainerHandler.cs     ← 容器整体拖拽
└── TurnPageHandler.cs      ← 翻页按钮点击

详情层
└── DetailFiller.cs         ← IDetailFiller 默认实现，异步加载 ItemTable + 自动定位

操作层
├── DragTool.cs             ← 拖拽幽灵图标 + 悬停阴影
├── SetItem.cs              ← 物品数据写入 + View 刷新 + NoView 隐藏
└── SetPage.cs              ← 翻页逻辑 + 最后一页高度适配 + 滑动动画反馈

工具层
├── RectUtility.cs          ← 程序化 RectTransform 创建
└── TaskSafeguard.cs        ← FireAndForget 异常捕获

测试
└── Test.cs                 ← 启动时随机填充物品 (EDITOR only, 1/3 概率)

Editor
├── ContainerSpecDrawer.cs  ← ContainerSpec 自定义折叠 Inspector
└── ItemDataEditorTools.cs  ← Project 右键 → Create → ItemTable
```

---

## 特性

- **零预制体** — 所有 UI 通过 `new GameObject` 程序化生成，Container → Mask → Grid → Cell 完整层级
- **多容器并存** — 一个 `Core` 组件，`specs[]` 数组驱动任意数量独立容器面板
- **跨容器交换** — 长按拖拽物品可在不同容器之间互换位置
- **Addressables 异步** — `ItemTable` 按需加载，30 分钟缓存过期自动回收
- **长按 + 边缘滚动** — 长按提取物品后拖到 Mask 边缘自动滚动列表，拖到左右边缘自动翻页
- **自定义 Inspector** — `ContainerSpec` 可折叠参数面板，新增元素自动填充默认值
- **两构建模式** — 纯数据驱动（`Build`）或预制体实例化（`BuildPrefab`），通过 `prefabRect` 是否为空切换
- **详情接口** — `IDetailFiller.Fill(container, itemKey)`，挂载到 `detailRect` 预制体即可

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
| 点击 Cell | 无长按则 Grid 拖拽滚动 |
| 长按 Cell (0.3s) | 提取物品到幽灵拖拽，原 Cell 隐藏 |
| 拖拽中命中 Cell | 悬停阴影显示，实时追踪 `targetItemKey` |
| 拖拽到 Mask 边缘 | 自动滚动 Grid |
| 拖拽到 Mask 左右边缘 | 自动翻页 |
| 松开手指 | 交换 source ↔ target 物品，UI 自动刷新 |
| 点击翻页按钮 | 上一页 / 下一页 |
| 点击页码输入框 | 输入数字跳页 |

---

## API

### Core（公开字段）

| 字段 | 类型 | 说明 |
|---|---|---|
| `specs` | `ContainerSpec[]` | 容器蓝图数组，每项生成一个独立容器 |
| `font` | `TMP_FontAsset` | 全局 TMP 字体 |
| `pressTime` | `float` | 长按判定时间（默认 0.3s） |
| `scrollSpeed` | `float` | 边缘滚动速度（默认 60） |
| `edgeThreshold` | `float` | 边缘触发距离（默认 3） |
| `turnThreshold` | `float` | 翻页冷却时间（默认 0.5s） |
| `shadowColor` | `Color` | 拖拽悬停阴影颜色（默认黑色半透明） |

### SetItem

```csharp
// 创建物品并自动刷新 UI（当前页时才刷新）
SetItem.Set(core, container, itemKey, id, type, tier, count, data);

// 从 Addressables 加载 ItemTable 并刷新单个 Cell
await SetItem.View(core, container, itemKey);

// 隐藏 Cell 显示，不触碰数据
SetItem.NoView(container, itemKey);
```

### SetPage

```csharp
// 跳转到指定页，刷新所有可见 Cell
SetPage.Set(core, container, page);
```

### TouchExchangeItem

```csharp
// 交换 source 和 target 位置的物品（支持跨容器）
TouchExchangeItem.Exchange(core);
```

### IDetailFiller

```csharp
public interface IDetailFiller
{
    void Fill(Container container, int itemKey);
}
```

将实现了 `IDetailFiller` 的组件挂在 `detailRect` 预制体上，点击物品时自动回调。

---

## 数据流

```
按下 Cell
  → Begin: 计算 sourceItemKey, 定位 sourceContainer
  → OnPointerDown: tag="Cell" → TouchCell.On (启动长按计时)
    → 长按触发: ExtractItem (拖拽幽灵 + NoView 隐藏原 Cell)
      → OnDrag: 每帧更新 targetRect / targetContainer / targetItemKey
        → TouchMask: 边缘滚动 + 翻页 (SetPage 后补 NoView)
    → 抬手: OnPointerUp
      → tag="Cell" → TouchCell.End → ExchangeItem (数据互换 + UI 刷新)
      → Reset: 在当前页则 View 恢复 source Cell, 清空全部状态
```

---

## License

MIT © 2026 覃健培 (Qin Jianpei)
