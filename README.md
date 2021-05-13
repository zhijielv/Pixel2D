# pixel2D Project-baozipi

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
     * Packages里的manifest.json添加"com.unity.entities": "0.11.2-preview.1",（包含ECS和burst等组件）；
     * 添加"com.unity.rendering.hybrid": "0.5.2-preview.4",（用于渲染Entity，否则不显示）；