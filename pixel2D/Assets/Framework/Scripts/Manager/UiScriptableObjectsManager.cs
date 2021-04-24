using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.Scripts.UI.ScriptableObjects;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.Scripts.Manager
{
    /// <summary>
    /// todo 测试打包
    /// 1. 设置view到address
    /// 2. 仅设置此资源打包，加载view
    /// </summary>
    [HideMonoScript]
    [SirenixGlobalConfig]
    [GlobalConfig("Art/ScriptableObject/UIScriptableObjectManager")]
    public class UiScriptableObjectsManager : GlobalConfig<UiScriptableObjectsManager>
    {
        [ShowInInspector] [ReadOnly] [ListDrawerSettings(Expanded = true)]
        public PanelScriptableObjectBase[] UiScriptableObjectsList;

        public bool isGenerateCode = false;

        [ReadOnly] public Object[] selectViews;

        // 刷新列表
#if UNITY_EDITOR
        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void ResetAllViewObjOverview()
        {
            UiScriptableObjectsList = AssetDatabase.FindAssets("t:PanelScriptableObjectBase")
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<PanelScriptableObjectBase>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
#endif


        public GameObject GetUiViewObj(string viewName)
        {
            foreach (PanelScriptableObjectBase objectBase in UiScriptableObjectsList)
            {
                if (objectBase.panelObj.name.Equals(viewName))
                {
                    return objectBase.panelObj;
                }
            }

            Debug.LogError("has not ViewSO");
            return null;
        }
    }
}