/*
** Created by fengling
** DateTime:    2021-04-21 13:50:16
** Description: 地图编辑器
*/

using System.Linq;
using Editor.Tools;
using Editor.Tools.TimelineTool;
using Editor.WaveFunctionCollapseEditor;
using Framework.Scripts.Constants;
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
        private const string _editorHelperAssetsPath = "Assets/Editor/LevelEditor/";
        private LevelTool _levelTool;
        private MapTool _mapTool;
        private AddressableAssetsTool _addressableAssetsTool;
        private LanguageTool _languageTool;
        // private WFCEditor _wfcEditor;
        private TimelineTool _timelineTool;

        protected override void OnEnable()
        {
            base.OnEnable();
            _levelTool = new LevelTool();
            _mapTool = new MapTool();
            _addressableAssetsTool = new AddressableAssetsTool();
            _languageTool = new LanguageTool();
            // _wfcEditor = new WFCEditor();
            _timelineTool = new TimelineTool();
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
            OdinMenuTree tree = new OdinMenuTree(true);
            
            // UI辅助工具
            tree.AddAssetAtPath("Ui Builder Setting",
                "Assets/Editor/Tools/UITool/UiBuilderSetting.asset");
            tree.AddAssetAtPath("Ui ScriptableObjects Manager", _configPath + "UiScriptableObjectsManager.asset");
            
            // 关卡配置工具
            tree.AddAssetAtPath("Level Helper", _editorHelperAssetsPath + "LevelHelper.asset");
            
            // 资源包工具
            tree.Add("Addressable Tool", _addressableAssetsTool);
            
            // 语言包工具
            tree.Add("Language Tool", _languageTool);
            
            //地图编辑工具
            // tree.Add("WFC Tool", _wfcEditor);
            tree.Add("WFCModule Tool", new WFCModuleCreator());
            tree.AddAssetAtPath("Project Common", _configPath + "Common.asset");
            tree.Add("Level Tool", _levelTool);
            tree.Add("Map Tool", _mapTool);
            tree.Add("Timeline Tool", _timelineTool);
            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            // base.OnBeginDrawEditors();
            var selected = MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = MenuTree.Config.SearchToolbarHeight;
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                // todo 重启Editor
                // if (SirenixEditorGUI.ToolbarButton((new GUIContent("重启Editor"))))
                // {
                //     Close();
                //     Show();
                // }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
#endif
}