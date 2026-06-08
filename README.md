# Item Manager

**多 Cell 容器资产管理框架** — 背包、仓库、装备槽、炼丹炉、锻造台… 所有需要格子系统的基础设施。

纯代码生成 UI，零预制体依赖。Addressables 异步加载资源。多容器并存。

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black)](package.json)

---

## 架构

```
Core.cs                     ← 触控分发中枢 (PointerDown/Drag/Up)
├── Core_Fields.cs          ← 公开字段 (specs[] / font / canvas)
├── Core_Init.cs            ← 启动 (Awake + Start)
└── Core_Addressables.cs    ← ItemTable 异步加载 + 缓存 + 过期回收

数据层
├── _Item.cs                ← Id / Type / Tier / Count / Data
├── _ItemTable.cs           ← ScriptableObject 物品表
├── _Cell.cs                ← 格子内 UI 组件引用
├── _Container.cs           ← 运行时容器实例
└── _ContainerSpec.cs       ← 容器蓝图 (尺寸/格子/翻页/视觉)

构建层
└── ___ContainerBuilder.cs  ← Container→Mask→Grid→Cell 全程序化构建

Editor
├── ContainerSpecDrawer.cs  ← ContainerSpec 自定义 Inspector
└── ItemDataEditorTools.cs  ← Project 右键快速创建 ItemTable
```

> 文件名下划线数量 = 层级优先级。在 Project 窗口中自然按层级排序。

---

## 特性

- **纯代码构建** — 无需任何 Prefab，所有 UI 程序化生成
- **多容器并存** — 一个 Core 组件，多个独立容器面板
- **Addressables** — ItemTable 异步加载，30 分钟过期缓存
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
3. 运行 → 自动构建容器 UI

```
specs[0]:
    totalItems  = 80           // 总物品数
    everyPageCells = 40        // 每页格子数
    row = 5                    // 每行格子数
    cellWidth = 10             // 格子边长
    maskHeight = 40            // 显示区域高度
```

---

## License

MIT © 2026 覃健培 (Qin Jianpei)
