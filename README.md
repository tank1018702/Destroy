# Destroy DEV 2.4.5

#### DE · Charlie Presents

### 介绍:
Destroy是一个Windows控制台游戏框架。

### 特点:
组件式设计, 提供游戏物体的生命周期（类似Unity）, 以及一系列相对应组件与常用工具。

### 目标:
Destroy的目标是 : 提供简单实用的工具帮助你快速, 轻松地制作Windows控制台游戏或2D游戏原型。（比如：塞尔达传说荒野之息的2D游戏原型, 贪吃蛇, 俄罗斯方块, 文字冒险类游戏）

### 现阶段计划:

Destroy已经从DE(原型开发)升级到了DEV(初期开发)阶段, 但依然缺乏一些核心系统的支持：

1. 渲染系统(需要重构)

2. 物理系统(需要重构)

3. 网络系统(基本完成)

4. AI寻路系统(暂未开放)

在之后的更新中会不断完善这些系统, 更新的优先级也从高到低排列。

欢迎提交分支!

### TODO:

1. 场景树

2. UI系统, UI组件

3. 可视化调试器

4. 新的基于字符串与C++API的渲染系统

5. 标准网络代码与局域网房间系统

6. AI寻路系统

7. 标准UI库与对话框

8. 扩展物理组件(物理碰撞集群)

9. 部分功能组件化

10. 外部工具整合(Porsche, protobuf tool)

11. 标准化设置

12. 数学库快速排序(实现)

### 已知Bug (提交bug: 1669247240@qq.com):

1. NetworkClient/NetworkSystem无法检测Connected状态

2. API访问级别

### API帮助:
null

### 示例项目:

https://github.com/ProcessCA/Boxhead

### 特别感谢 & 参考(排名不分先后):

https://unity3d.com

https://github.com/ollelogdahl/ConsoleGameEngine

https://github.com/Easycker/ConsoleGameFramework

https://github.com/KristianBalaj/2d-console-game-engine

https://github.com/MaxMls/WindowsConsoleGame

https://github.com/jilleJr/YummyConsole