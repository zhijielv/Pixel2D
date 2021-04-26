/*
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
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Utilities;
using UnityEngine;

namespace Editor.Tools
{
#if UNITY_EDITOR
    public class AddressableAssetsTool
    {
        #region AddressableAssetsTool

        [MenuItem("Assets/FrameWork View/Add All View To Addressables Groups", false, -2)]
        public static void Add2AddressablesGroups()
        {
            GlobalConfig<UiScriptableObjectsManager>.Instance.ResetAllViewObjOverview();
            List<PanelScriptableObjectBase> list =
                GlobalConfig<UiScriptableObjectsManager>.Instance.UiScriptableObjectsList.ToList();
            Add2AddressablesGroupsByName(list, "UIAssets");

            List<GameObject> viewObjects = list.Select(objectBase => objectBase.panelObj).ToList();
            Add2AddressablesGroupsByName(viewObjects, "UIView");
        }

        private static void Add2AddressablesGroupsByName<T>(List<T> srcList, string addressablesGropsName)
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

        #endregion
    }
#endif
}