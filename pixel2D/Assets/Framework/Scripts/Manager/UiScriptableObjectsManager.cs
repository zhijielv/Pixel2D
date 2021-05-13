using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.Scripts.UI.ScriptableObjects;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Framework.Scripts.Manager
{
    [HideMonoScript]
    [SirenixGlobalConfig]
    public class UiScriptableObjectsManager : GlobalConfig<UiScriptableObjectsManager>
    {
        [ShowInInspector] [ReadOnly] public PanelScriptableObjectBase[] UiScriptableObjectsList;

        [ShowInInspector] [ReadOnly] public Object[] UIPrefabs;

        // 刷新列表
#if UNITY_EDITOR
        [Button("获取所有SO", ButtonSizes.Medium), PropertyOrder(-1)]
        public void ResetAllViewSO()
        {
            UiScriptableObjectsList = AssetDatabase.FindAssets("t:PanelScriptableObjectBase")
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<PanelScriptableObjectBase>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
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

            UIPrefabs = tmpObjList.ToArray();
        }
#endif
        public PanelScriptableObjectBase GetUiViewSo(string viewName)
        {
            foreach (PanelScriptableObjectBase objectBase in UiScriptableObjectsList)
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
            foreach (var uiPrefab in UIPrefabs)
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