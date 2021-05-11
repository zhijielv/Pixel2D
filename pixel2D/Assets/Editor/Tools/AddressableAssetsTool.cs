﻿/*
** Created by fengling
** DateTime:    2021-04-25 11:36:57
** Description:
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.Scripts.Manager;
using Framework.Scripts.UI.ScriptableObjects;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Tools
{
#if UNITY_EDITOR
    public class AddressableAssetsTool
    {
        #region AddressableAssetsTool

        [ShowInInspector]
        private string _addressablesGropsName = "";
        
        [Button("添加选中文件夹下所有资源", ButtonSizes.Large)]
        public void AddSelectFolderAssets2AddressGroup()
        {
            Object[] objects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            if(objects.Length == 0) return;
            List<string> tmpFolder = new List<string>();
            foreach (Object o in objects)
            {
                string assetPath = AssetDatabase.GetAssetPath(o);
                if (Directory.Exists(assetPath))
                {
                    tmpFolder.Add(assetPath);
                }
            }

            string[] findAssets = AssetDatabase.FindAssets("t:Object", tmpFolder.ToArray());
            List<Object> tmpObjs = new List<Object>();
            foreach (string findAsset in findAssets)
            {
                if(Directory.Exists(AssetDatabase.GUIDToAssetPath(findAsset))) continue;
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(findAsset));
                tmpObjs.Add(obj);
            }

            Add2AddressablesGroupsByName(tmpObjs, _addressablesGropsName);
        }
        
        [Button("添加所有UI资源", ButtonSizes.Large)]
        [MenuItem("Assets/FrameWork View/Add All View To Addressables Groups", false, -2)]
        public static void Add2AddressablesGroups()
        {
            GlobalConfig<UiScriptableObjectsManager>.Instance.ResetAllViewPrefab();
            List<Object> list =
                GlobalConfig<UiScriptableObjectsManager>.Instance.UIPrefabs.ToList();
            Add2AddressablesGroupsByName(list, "UIAssets");

            List<PanelScriptableObjectBase> viewObjects = AssetDatabase.FindAssets("t:PanelScriptableObjectBase")
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<PanelScriptableObjectBase>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
            Add2AddressablesGroupsByName(viewObjects, "UIView");
        }

        public static void Add2AddressablesGroupsByName<T>(List<T> srcList, string addressablesGropsName)
            where T : UnityEngine.Object
        {
            AddressableAssetGroup findGroup =
                AddressableAssetSettingsDefaultObject.Settings.FindGroup(addressablesGropsName);
            if (findGroup != null)
            {
                AddressableAssetSettingsDefaultObject.Settings.RemoveGroup(findGroup);
            }
            
            findGroup = AddressableAssetSettingsDefaultObject.Settings.CreateGroup(addressablesGropsName, false, false, false,
                null);
            findGroup.AddSchema<BundledAssetGroupSchema>();
            findGroup.AddSchema<ContentUpdateGroupSchema>();
            foreach (T t in srcList)
            {
                string path = AssetDatabase.GetAssetPath(t);
                if (IsPathValidForEntry(path))
                {
                    var guid = AssetDatabase.AssetPathToGUID(path);
                    if (!string.IsNullOrEmpty(guid))
                    {
                        if (AddressableAssetSettingsDefaultObject.Settings == null)
                            AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create(
                                AddressableAssetSettingsDefaultObject.kDefaultConfigFolder,
                                AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);
                        Undo.RecordObject(AddressableAssetSettingsDefaultObject.Settings, "AddressableAssetSettings");
                        AddressableAssetEntry entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(
                            guid,
                            AddressableAssetSettingsDefaultObject.Settings.FindGroup(addressablesGropsName));
                        string s = Path.GetFileNameWithoutExtension(entry.address);
                        entry.address = s;
                    }
                }
            }
        }

        static HashSet<string> excludedExtensions =
            new HashSet<string>(new string[] {".cs", ".js", ".boo", ".exe", ".dll", ".meta"});

        internal static bool IsPathValidForEntry(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            if (!path.StartsWith("assets", StringComparison.OrdinalIgnoreCase) && !IsPathValidPackageAsset(path))
                return false;
            if (path == CommonStrings.UnityEditorResourcePath ||
                path == CommonStrings.UnityDefaultResourcePath ||
                path == CommonStrings.UnityBuiltInExtraPath)
                return false;
            return !excludedExtensions.Contains(Path.GetExtension(path));
        }

        internal static bool IsPathValidPackageAsset(string path)
        {
            string convertPath = path.ToLower().Replace("\\", "/");
            string[] splitPath = convertPath.Split('/');

            if (splitPath.Length < 3)
                return false;
            if (splitPath[0] != "packages")
                return false;
            if (splitPath.Length == 3)
            {
                string ext = Path.GetExtension(splitPath[2]);
                if (ext == ".json" || ext == ".asmdef")
                    return false;
            }

            return true;
        }
        
        [Button("添加所有Json资源", ButtonSizes.Large)]
        public static void AddAllJson2AddressGroup()
        {
            AssetDatabase.Refresh();
            List<TextAsset> textAssets = AssetDatabase.FindAssets("t:TextAsset", new[] {"Assets/Framework/Json"})
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
            AddressableAssetsTool.Add2AddressablesGroupsByName(textAssets, "Json");
        }

        #endregion

        // todo 设置打包
        [Button("打包", ButtonSizes.Large)]
        public void BuildAssets()
        {
            
        }
    }
#endif
}