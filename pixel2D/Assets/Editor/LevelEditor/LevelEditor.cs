/*
** Created by fengling
** DateTime:    2021-04-21 13:50:16
** Description: 地图编辑器
*/

using Editor.Tools;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor.LevelEditor
{
#if UNITY_EDITOR

    public class LevelEditor : OdinMenuEditorWindow
    {
        private const string _configPath = "Assets/Plugins/Sirenix/Odin Inspector/Config/Resources/Sirenix/";
        private LevelTool _levelTool;
        private MapTool _mapTool;
        private AddressableAssetsTool _addressableAssetsTool;

        protected override void OnEnable()
        {
            base.OnEnable();
            _levelTool = new LevelTool();
            _mapTool = new MapTool();
            _addressableAssetsTool = new AddressableAssetsTool();
        }

        [MenuItem("Tools/Level Editor %Q")]
        private static void OpenWindow()
        {
            var window = GetWindow<LevelEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
            window.titleContent = new GUIContent("Level Editor");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(true)
            {
                {"Level Tool", _levelTool},
                {"Map Tool", _mapTool},
                {"Addressable Tool", _addressableAssetsTool},
            };
            tree.AddAssetAtPath("UiBuilderSetting",
                "Assets/Editor/Tools/UITool/UiBuilderSetting.asset");
            tree.AddAssetAtPath("UiScriptableObjectsManager", _configPath + "UiScriptableObjectsManager.asset");
            tree.AddAssetAtPath("ProjectCommon", _configPath + "Common.asset");
            return tree;
        }
    }
#endif
}