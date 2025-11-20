
##  游戏特性
<img width="1583" height="849" alt="微信图片_20251120154737_47_6" src="https://github.com/user-attachments/assets/6472833f-625c-42d9-af79-6bfd4e0a9efa" />
- **完整的Roguelike循环**：随机地图、随机事件、回合制战斗
  <img width="1417" height="832" alt="微信图片_20251120154816_48_6" src="https://github.com/user-attachments/assets/adaae51f-4057-48c6-81bf-3e75f2b26aad" />
- **模块化卡牌系统**：支持效果组合的状态机架构
- <img width="1546" height="885" alt="微信图片_20251120155003_49_6" src="https://github.com/user-attachments/assets/996f8e7b-62a4-47ff-a254-f7ada432402b" />

- **数据驱动设计**：使用ScriptableObject实现可配置的游戏平衡
- **性能优化**：对象池管理、事件驱动通信

##  技术架构

### 核心系统
- **事件驱动架构**：基于ScriptableObject的自定义事件系统
- **藏品组件**：可扩展的效果组合系统
- **UI管理系统**：基于MVC模式的界面管理

### 性能优化
- 使用对象池管理卡牌实例
- 事件系统减少组件耦合
- 异步加载和资源管理
