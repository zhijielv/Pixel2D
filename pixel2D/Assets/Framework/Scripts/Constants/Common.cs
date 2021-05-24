using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Framework.Scripts.Constants
{
    public class Common : GlobalConfig<Common>
    {
        [ShowInInspector] public static float Tolerance = 0.01f;
        // Debug模式
        [ShowInInspector] public static bool IsDebugMode = true;

        // 帧率
        [ShowInInspector] public static int FrameRate;

        // Launch初始化
        [ShowInInspector] public static bool Initialized = false;

        // 单格子尺寸
        [ShowInInspector] public static float TileSize = 0.16f;

        // 地图缩放
        [ShowInInspector] public static int LevelManagerScale = 3;

        ////////////////////////////////////////////  GameObject  ///////////////////////////////////////////////////////
        [ShowInInspector] public static GameObject MainCanvas;
        [ShowInInspector] public static GameObject RewiredInputManager;
        [ShowInInspector] public static GameObject FrameWorkObj;
    }
}