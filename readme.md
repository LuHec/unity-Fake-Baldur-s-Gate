#### 更新日志

##### 2023/10/11 

- 角色添加到格子系统中，实现移动和格子分离。
- 地图、管理、命令中心基础框架搭建完成

##### 2023/10/12

- 回合制框架初步搭建

##### 2023/10/13

- 完成回合制框架
- A*寻路

##### 2023/10/14

- 启动结束回合制模式
- 战斗功能初步搭建
- 镜头控制
- 人物数据表格加载

##### 2023/10/15

- 加入id池，重构资源管理代码
- 武器加载完成
- 攻击基础框架完成

##### 2023/10/16

- 加入音频
- 攻击特效，炸弹，子弹，攻击具体逻辑
- 优化id池，加入回收机制
- 事件监控：用观察订阅模式记录玩家的行为，其他用户订阅感兴趣的部分
- 镜头晃动
- MessageCenter重组回合

##### 2023/10/17

- 修改模块消息回调

- 把ai模块和输入模块挂载在角色身上，重构回合执行模式
- 修复MessageCenter重组回合😇 😇 😇 😇 😇 😇 😇 😇 😇 😇 😇 😇 😇 （已经肝硬化）
- 并行战斗处理完毕
- 死亡退出回合，清除地图，退出管理器，退出ID池
- 地图存档基本

##### 2023/10/18

- 完成回合结束自动清理角色，移动到自由列表
- 敌人AI添加攻击
- 完成自由模式NPC AI
- 完成自由列表存储非回合制人员
- 完成主角跟随

##### 2023/12/9

- 修改地图，不存角色了，改成物理检测交互

##### 2023/12/10

- 完成UI框架

- 发现UI交互和控制命令产生的问题

  - 修改方案如下：

    - ui交互的指令无法推进回合

      把IO指令交给总的CommandCenter，让角色自己去取指令

    - 非控制角色回合产生的命令被取走

      只有当前控制角色不在回合内，或者轮到当前控制角色时，才能产生指令

    - 拆开队伍导致多个角色收到相同指令

      用一个总的类接受玩家的IO指令，只有当TurnInstance的当前角色是被控制的角色，才能从控制类里获取指令，否则一直阻塞

##### 2023/12/11

- 修复了UI和回合制框架
  - 添加Actor循环，处理AI
  - 单独脚本处理玩家输入，会判定条件添加指令到当前控制的角色身上
  - 同时修复了多个控制角色收到相同指令的问题(脚本只对单个角色输入)
  - 回合管理器只取指令，执行指令，清除指令。不再干涉角色本身
- 修复了死亡后还能攻击的bug

##### 2023/12/14

- 修改了回合制框架，加入了任务队列机制
- 添加了buff框架，现在可以每个回合结算buff了

##### 2023/12/16

- 加入了gas系统
  - Modifer
  - ABility
  - Attribute Set
- 移除了buff系统
- 修改了Command触发逻辑
  - 改成了只触发一次的协程异步执行
  - 出现了bug，自由模式下只能限制命令产生
    - 处理方法是写一个中断函数，暂时搁置

##### 2023/12/18

- 舍弃命令模式（现在的框架写错了，命令模式只应该记录操作，而不是用来运行的）
  - 直接依靠Ability进行回调
  - Command只负责记录，回放时解析Command调用即可
- 添加技能选择器，直接写在技能里面。
  - 外部激活时监听，如果技能执行了进行广播；取消执行则不进行广播，这样可以防止取消选中时也推进回合
  - ui只需要激活能力即可，剩下交给技能自己执行

