using UnityEngine;

namespace Framework.Scripts.Constants
{
    public class Common
    {
        // 帧率
        public static int FrameRate;
        // Launch初始化
        public static bool Initialized = false;
        // 单格子尺寸
        public static float TileSize = 0.16f;
        // 地图缩放
        public static int LevelManagerScale = 3;
        
        ////////////////////////////////////////////  GameObject  ///////////////////////////////////////////////////////
        public static GameObject MainCanvas;
        public static GameObject RewiredInputManager;
        public static GameObject FrameWorkObj;
    }
}