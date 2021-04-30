﻿using Editor.Tools;
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
        private LevelTool _levelTool;
        private AddressableAssetsTool _addressableAssetsTool;

        protected override void OnEnable()
        {
            base.OnEnable();
            _levelTool = new LevelTool();
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
                {"Addressable Tool", _addressableAssetsTool},
            };
            return tree;
        }
    }
#endif
}