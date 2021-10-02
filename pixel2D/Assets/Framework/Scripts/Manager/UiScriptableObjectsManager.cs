using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.Scripts.UI.ScriptableObjects;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Framework.Scripts.Manager
{
    [HideMonoScript]
    [SirenixGlobalConfig]
    public class UiScriptableObjectsManager : GlobalConfig<UiScriptableObjectsManager>
    {
        [FormerlySerializedAs("UiScriptableObjectsList")] [ShowInInspector] [ReadOnly] public PanelScriptableObjectBase[] uiScriptableObjectsList;

        [FormerlySerializedAs("UIPrefabs")] [ShowInInspector] [ReadOnly] public Object[] uiPrefabs;

        // 刷新列表
#if UNITY_EDITOR
        [Button("获取所有SO", ButtonSizes.Medium), PropertyOrder(-1)]
        public void ResetAllViewSo()
        {
            uiScriptableObjectsList = AssetDatabase.FindAssets("t:PanelScriptableObjectBase")
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<PanelScriptableObjectBase>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
            foreach (PanelScriptableObjectBase objectBase in uiScriptableObjectsList)
            {
                string uiName = objectBase.name.Split(new[] {"_Asset"}, StringSplitOptions.RemoveEmptyEntries)[0];
                foreach (var o in uiPrefabs)
                {
                    if (!o.name.Equals(uiName)) continue;
                    objectBase.panelObj = o as GameObject;
                    break;
                }
            }
        }

        [Button("获取所有UI", ButtonSizes.Medium), PropertyOrder(-2)]
        public void ResetAllViewPrefab()
        {
            string srcPath = Constants.Constants.ViewPrefabDir;
            List<Object> tmpObjList = new List<Object>();
            string tmpName = "";
            foreach (string path in Directory.GetFiles(srcPath, "*.prefab", SearchOption.AllDirectories))
            {
                tmpName = path;
                tmpName = Directory.Exists(path) ? Path.GetDirectoryName(path) : Path.GetFileNameWithoutExtension(path);
                if (tmpName.EndsWith("_View"))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(System.Object)) as GameObject;
                    tmpObjList.Add(prefab);
                }
            }

            uiPrefabs = tmpObjList.ToArray();
        }
#endif
        public PanelScriptableObjectBase GetUiViewSo(string viewName)
        {
            foreach (PanelScriptableObjectBase objectBase in uiScriptableObjectsList)
            {
                if (objectBase.name.Equals(viewName))
                {
                    return objectBase;
                }
            }

            Debug.LogError("has not ViewSO " + viewName);
            return null;
        }

        public GameObject GetUiViewObj(string viewName)
        {
            foreach (var uiPrefab in uiPrefabs)
            {
                if (uiPrefab.name.Equals(viewName))
                {
                    return (GameObject) uiPrefab;
                }
            }

            Debug.LogError("has not ViewObj");
            return null;
        }
    }
}