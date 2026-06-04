# Item Manager — 项目开发指南

---

## 1. 项目概览

**Item Manager** 是一个 Unity 通用容器资产管理框架，专为背包、仓库、装备槽、炼丹炉、锻造台等多 Cell 容器系统设计。它提供了网格生成、物品数据管理、拖拽触控以及跨容器交互的完整基础设施。

| 属性 | 说明 |
|------|------|
| 包名 | `com.lookloop.item-manager` |
| 命名空间 | `Lookloop.ItemManager` |
| Unity 版本 | 2022.3 |
| 语言 | C# |
| 关键依赖 | Addressables 1.21.0、TextMeshPro 3.0.0、Unity uGUI 1.0.0 |

### 核心设计理念

- **Tag 驱动路由**：`Core` 组件通过 GameObject 的 tag（`"Cell"` / `"Container"`）区分 Item 交互与 Container 交互
- **距离判定拖拽**：不使用 Unity 原生 `IBeginDragHandler`，而是用起手坐标与实际坐标的差值判定是否拖拽
- **蓝图构建**：`ContainerSpec` 作为容器蓝图，在 `Core` 的 `specs` 数组中填入多少项就生成多少个独立容器
- **预制体优先**：优先使用外部预制体实例化容器；无预制体时按参数代码生成完整 UI 层级

---

## 2. 快速上手

### 2.1 环境要求

- Unity 2022.3 或更高版本
- 已安装 Addressables 包（`com.unity.addressables`）
- 已安装 TextMeshPro（`com.unity.textmeshpro`）
- 项目使用 Unity uGUI

### 2.2 安装

通过 Unity Package Manager 从 Git 或本地路径安装本包。包名：`com.lookloop.item-manager`

### 2.3 基础用法

1. **在 Canvas 下挂载 `Core` 组件**
   - 创建一个空 GameObject，重命名为 `ItemManager`
   - 将 `Core` 脚本挂载到该对象上

2. **配置容器蓝图**
   - 在 `Core` 组件的 `specs` 数组中添加 `ContainerSpec` 项
   - 可选：拖入预制体（自定义容器外观），或留空让框架自动生成

3. **运行**
   - 运行时 `Core.Start()` 会调用 `ContainerBuilder.BuildAll()` 自动构建所有容器

### 2.4 创建物品表数据

在 Project 窗口中右键 → `Assets/Create/ItemManager/ItemData`（或使用菜单 `Assets/Create/ItemTable（脚本创建）`）来创建新的 `ItemTable` ScriptableObject 资源。

### 2.5 加载物品表

使用 Addressables 异步加载：

```csharp
Core core = GetComponent<Core>();
ItemTable table = await core.GetItemTable("YourItemTableKey");
```

---

## 3. 项目结构

```
ItemManager/
├── package.json                    # 包描述文件
├── Runtime/
│   ├── ItemManager.asmdef          # 运行时程序集定义
│   ├── Core.cs                     # 主入口（IPointerDownHandler/IDragHandler/IPointerUpHandler）
│   ├── Core_Init.cs                # Core 的初始化逻辑（Awake/Start）
│   ├── Core_Fields.cs              # Core 的字段（specs/canvas/PointerDownTag）
│   ├── Core_Addressables.cs        # ItemTable 的 Addressables 加载与缓存
│   ├── _ContainerSpec.cs           # 容器蓝图数据类
│   ├── _ContainerMod.cs            # 容器运行时模块（Transform + 物品数据 + 蓝图引用）
│   ├── _Item.cs                    # 物品数据类
│   ├── _ItemTable.cs               # 物品表 ScriptableObject
│   ├── __ContainerTouch.cs         # Container 面板拖拽逻辑
│   ├── __ItemTouch.cs              # Item 触控逻辑（占位，待实现）
│   ├── ___ContainerBuilder.cs      # 容器 UI 构建器（生成 Container→Mask→Grid→Cell 层级）
│   ├── ___ContainerManager.cs      # 容器注册/管理
│   ├── ___ItemDataManager.cs       # 物品数据管理（占位）
│   └── ___ItemsController.cs       # 物品控制器（翻页/刷新/创建）
├── Editor/
│   ├── ItemManager.Editor.asmdef   # 编辑器程序集定义
│   ├── ContainerSpecDrawer.cs      # ContainerSpec 的自定义 Inspector 面板
│   └── ItemDataEditorTools.cs      # 编辑器扩展（快速创建 ItemTable 资源）
└── .continue/
    └── rules/
        └── CONTINUE.md             # 本文件
```

### 文件命名约定

| 前缀 | 含义 | 示例 |
|------|------|------|
| `Core` | 主入口文件（partial class） | `Core.cs`, `Core_Init.cs` |
| `_` | 数据类 / 蓝图类 | `_Item.cs`, `_ContainerSpec.cs` |
| `__` | 触控逻辑静态类 | `__ContainerTouch.cs`, `__ItemTouch.cs` |
| `___` | 管理器 / 构建器静态类 | `___ContainerBuilder.cs`, `___ItemsController.cs` |

> ⚠️ 下划线数量越多，表示该模块离 Core 越远，职责越独立。

---

## 4. 核心概念

### 4.1 两大交互路由

| Tag | 对应触控类 | 职责 |
|-----|-----------|------|
| `"Cell"` | `ItemTouch` | 物品格的点击/拖拽/交换/长按/详情 |
| `"Container"` | `ContainerTouch` | 容器面板的拖拽移动 |

其他 tag 的 GameObject 将被 `Core.OnPointerDown` 忽略。

### 4.2 触控阶段流程

```
A 阶段 — OnPointerDown（起手）
    ↓
C 阶段 — OnDrag（拖拽中，每帧）
    ↓
D 阶段 — OnPointerUp（结算）
```

- 仅响应 `pointerId == 0`（单指触控）
- 拖拽判定采用**距离阈值**（sqrMagnitude > 0.01f），非 Unity BeginDrag 事件

### 4.3 容器 UI 层级

```
Container (tag="Container", 有 Image)
├── Mask (有 Mask 组件, Image)
│   └── Grid (tag="Grid")
│       └── Cell[0..N] (tag="Cell", 有 Image)
├── PageText (tag="TurnPage", 含 TMP 文本)
│   ├── PrevButton (tag="TurnPage")
│   └── NextButton (tag="TurnPage")
└── Detail (可选，外部传入)
```

- `ContainerBuilder.Build()` 从 `ContainerSpec` 参数自动生成以上层级
- `ContainerBuilder.BuildFromPrefab()` 使用外部预制体，自动扫描 `tag="Cell"` 的子对象

### 4.4 物品数据结构

```csharp
[Serializable]
public class Item
{
    public int Id;      // 唯一ID
    public int Type;    // 类型
    public int Tier;    // 等级/品阶
    public int Count;   // 数量
    public int[] Data;  // 扩展数据
}
```

### 4.5 容器数据模型

- `ContainerSpec`：编辑时配置的蓝图（每页格子数、行数、格子大小等）
- `ContainerMod`：运行时模块（包含 Transform 引用、物品数组、当前页码）
- `ContainerManager.containers`：全局容器注册表（`static List<ContainerMod>`）

---

## 5. 开发工作流

### 5.1 当前开发状态

| 模块 | 状态 | 说明 |
|------|------|------|
| `Core` / Core 路由 | ✅ 已完成 | Tag 路由 + 三阶段触控 |
| `ContainerTouch` | ✅ 已完成 | 面板拖拽已实现 |
| `ItemTouch` | ⚠️ 占位 | 类已定义，逻辑未实现；Core 中对应路由被标记为 "临时重构" |
| `ContainerBuilder` | ✅ 已完成 | 代码生成 + 预制体实例化均已实现 |
| `ContainerManager` | ✅ 已完成 | 容器注册表 |
| `ItemDataManager` | ⚠️ 占位 | 空类 |
| `ItemsController` | 🚧 进行中 | 基础翻页/刷新框架已搭建，细节待完善 |
| `Core_Addressables` | ✅ 已完成 | 带 30 分钟缓存回收的异步加载 |
| 编辑器扩展 | ✅ 已完成 | 自定义 Inspector + 快速创建资源 |

### 5.2 如何添加新功能

1. **新增物品数据类型**：修改 `_Item.cs` 添加字段；同步更新 `ItemsController` 的刷新逻辑
2. **新增容器行为**：在 `___ContainerManager.cs` 中添加静态方法
3. **扩展 Item 交互**：在 `__ItemTouch.cs` 中实现对应方法，并在 `Core.cs` 的路由中取消注释

### 5.3 编码约定

- 所有类位于命名空间 `Lookloop.ItemManager`
- 编辑器代码位于 `Lookloop.ItemManager.Editor`
- 使用 `[HideInInspector]` 序列化运行时字段（如 `canvas`, `PointerDownTag`）
- 静态工具类使用 `public static` 方法

### 5.4 测试方法

1. 在 Unity 场景中创建 Canvas
2. 挂载 `Core` 组件并配置 `specs` 数组
3. 运行场景，观察容器是否正确生成
4. 使用 Unity EventSystem 模拟触控事件测试拖拽逻辑

---

## 6. 常见任务

### 6.1 创建新容器

在 `Core` 组件的 `specs` 数组中新增一个 `ContainerSpec` 元素，配置参数即可。运行时自动生成。

### 6.2 自定义容器外观

1. 制作包含 `tag="Cell"` 子对象的预制体
2. 将预制体拖入 `ContainerSpec.prefab` 字段
3. 运行时框架会实例化该预制体并自动注册所有 Cell

### 6.3 给物品格子填充数据

```csharp
ItemsController.NewItem(containermod, itemKey, id, type, tier, count, data);
ItemsController.RefreshItem(containermod, itemKey);
```

### 6.4 创建物品表资源

- 方法一：Project 窗口右键 → `Create/ItemManager/ItemData`
- 方法二：顶部菜单 `Assets/Create/ItemTable（脚本创建）`

### 6.5 加载物品表

```csharp
ItemTable table = await core.GetItemTable("AssetKey");
if (table != null)
{
    // 使用 table.Id, table.ItemName, table.ItemSprite 等
}
```

---

## 7. 配置参数参考

### ContainerSpec 参数说明

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `prefab` | null | 不为空则使用预制体实例化 |
| `detail` | null | 物品详情面板引用 |
| `totalCells` | 80 | 物品总槽位数 |
| `everyPageTotal` | 40 | 每页显示的格子数 |
| `rows` | 5 | 每行格子数 |
| `cellWidth` | 10 | 格子边长（单位） |
| `maskHeight` | 40 | 遮罩高度 |
| `containerFillHorizontal` | 2 | 水平内边距 |
| `containerFillUp` | 8 | 上边距 |
| `containerFillDown` | 4 | 下边距 |
| `containerSprite` | null | 容器背景图 |
| `maskSprite` | null | 遮罩区域图 |
| `cellSprite` | null | 默认格子图 |

---

## 8. 故障排除

### 8.1 容器没有生成

- 检查 `specs` 数组是否至少有一个非空元素
- 确保 `Core` 组件挂载在 Canvas 下的 GameObject 上
- 检查 Console 是否有错误信息

### 8.2 ItemTable 加载失败

- 确认资源已正确标记为 Addressable
- 检查传入的 key 是否与 Addressables 组中的 key 一致
- 查看 Console 中的 `[Core] ItemTable 加载失败` 日志

### 8.3 拖拽无响应

- 检查 `Core` 所在 Canvas 是否正确设置了 `worldCamera`（尤其是 World Space 模式）
- 确认 EventSystem 存在于场景中
- 验证拖拽目标的 tag 是 `"Cell"` 或 `"Container"`

### 8.4 编辑器 Inspector 中 ContainerSpec 参数显示异常

- `ContainerSpecDrawer` 在数组新增元素时会自动填充默认值（检测 `totalCells == 0` 时写入）
- 如果参数显示为 0，展开"参数"折叠面板查看并手动调整

---

## 9. 参考资料

| 资源 | 链接/说明 |
|------|-----------|
| Unity Addressables 文档 | https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/index.html |
| Unity uGUI 文档 | https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/ |
| TextMeshPro 文档 | https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html |
| 项目仓库 | https://github.com/lookloop |

---

> 📝 **注意**：本文件由代码分析自动生成。请团队成员根据实际项目状况审核并补充以下内容：
> - 具体游戏中的物品类型枚举
> - Item 的 `Data[]` 数组各下标含义
> - 跨容器交互的具体业务规则
> - 性能优化的实践建议
> - 贡献指南和代码审查流程