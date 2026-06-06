# Item Manager

**多 Cell 容器资产管理框架** — 背包、仓库、装备槽、炼丹炉、锻造台… 所有需要格子系统的基础设施。

纯代码生成 UI，零预制体依赖。Addressables 异步加载 Sprite。多容器并存。拖拽、翻页、边缘检测滚动。

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black)](package.json)

---

## 架构

```
Core.cs                     ← 触控分发中枢 (PointerDown/Drag/Up)
├── Core_Fields.cs          ← 公开字段 (specs[] / font / canvas)
├── Core_Init.cs            ← 启动 (Awake + Start)
└── Core_Addressables.cs    ← Sprite 异步加载 + 缓存 + 过期回收

数据层
├── _Item.cs                ← Id / Type / Tier / Count / Data
├── _ItemTable.cs           ← ScriptableObject 物品表
├── _ItemUI.cs              ← Cell 内 UI 组件引用
├── _ContainerSpec.cs       ← 容器蓝图 (尺寸/格子/翻页/视觉)
└── _ContainerMod.cs        ← 运行时容器实例

构建层
├── ___ContainerBuilder.cs  ← Container→Mask→Grid→Cell 全程序化构建
├── ___ContainerManager.cs  ← 全局容器注册表
├── ___ItemsController.cs   ← SetItem / RemoveItem / SwapItem
└── ___MiscInit.cs          ← 拖拽跟手浮层

触控层
├── __CellTouch.cs          ← 短按拖网格 / 长按拖物品
├── __CellTouch_Drag.cs     ← 拖拽跟手 + 边缘检测翻页
├── __CellTouch_Grid.cs     ← 网格滚动
├── __ContainerTouch.cs     ← 容器面板拖拽
└── __TurnPageTouch.cs      ← 翻页按钮 + 页码输入
```

> 文件名下划线数量 = 层级优先级。在 Project 窗口中自然按层级排序。

---

## 特性

- **纯代码构建** — 无需任何 Prefab，所有 UI 程序化生成
- **多容器并存** — 一个 Core 组件，多个独立容器面板
- **Addressables** — Sprite 异步加载，30 分钟过期缓存
- **拖拽翻页** — 拖拽物品到边缘 1 秒自动翻页
- **面板拖拽** — 容器窗口可自由拖动位置
- **翻页输入** — 直接输入页码跳转
- **自定义 Inspector** — ContainerSpec 可折叠参数面板
- **快速创建** — Project 右键 `Create → ItemTable` 新建物品表

---

## 安装

### Unity Package Manager (Git URL)

```
https://github.com/lookloop/ItemManager.git
```

### 依赖

- `com.unity.addressables` ≥ 1.21.0
- `com.unity.textmeshpro` ≥ 3.0.0
- `com.unity.ugui` ≥ 1.0.0

---

## 快速开始

1. Canvas 下新建空 GameObject，挂 `Core` 组件
2. 在 `specs` 数组中填容器蓝图参数
3. 运行 → 自动构建容器 + 填充测试数据

```
specs[0]:
    totalCells = 80          // 总物品数
    everyPageTotal = 40      // 每页格子数
    rows = 5                 // 每行格子数
    cellWidth = 10           // 格子边长
    maskHeight = 40          // 显示区域高度
```

---

## 你的项目集成

```csharp
// 写入物品
ItemsController.SetItem(core, mod, itemKey, id, type, tier, count, data);

// 移除物品
ItemsController.RemoveItem(core, mod, itemKey);

// 交换物品
ItemsController.SwapItem(core, mod, keyA, keyB);

// 翻页
TurnPageTouch.NextPage(core, mod);
TurnPageTouch.PrevPage(core, mod);

// 遍历所有容器
foreach (var mod in ContainerManager.containers) { ... }
```

---

## 商业集成服务

需要将此框架集成到你的游戏项目？作者提供付费集成服务：

- 接入你的物品数据结构
- 自定义 UI 皮肤适配
- 装备比较 / 批量操作 / 交易系统扩展
- 存档系统对接

📧 联系：3357709076@qq.com

---

## License

MIT © 2026 覃健培 (Qin Jianpei)
