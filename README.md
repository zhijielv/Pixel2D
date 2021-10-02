# pixel2D Project-baozipi
keystore
密码：baozipi

2021-3-22 启动

# 用到的插件：
1. TileWorldCreator
   * 随机地图生成

2. Behavior Designer
   * 行为树

3. TopDownEngine（项目中移除）
    * 所需功能基本齐全


4. Rewired
   * 官方文档
https://guavaman.com/projects/rewired/docs/QuickStart.html
   * 用于检测并切换不同输入设备

5. DOTween
 * 动画插件


6. ECS
   * 目前只有preview版本
   * 导入方式：
     * Packages里的manifest.json添加:
     * "com.unity.burst": "1.4.8",
     * "com.unity.entities": "0.17.0-preview.42",
     * "com.unity.rendering.hybrid": "0.11.0-preview.44",
     * "com.havok.physics": "0.6.0-preview.3",
1. QHierarchy
   * 版本：v4.4
   * 用于Hierarchy操作；
   * 预制体模式下会一直自动保存，仔Tools->QHierarchy->Settings里关闭Lock取消刷新；