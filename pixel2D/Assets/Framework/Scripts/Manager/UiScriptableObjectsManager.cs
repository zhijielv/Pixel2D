using System;
using System.Collections.Generic;
using System.Linq;

using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UI.ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.Manager
{
    [HideMonoScript]
    [SirenixGlobalConfig]
    [GlobalConfig("Art/UIScriptableObjectManager")]
    public class UiScriptableObjectsManager : GlobalConfig<UiScriptableObjectsManager>
    {
        [ShowInInspector]
        [ReadOnly]
        [ListDrawerSettings(Expanded = true)]
        public PanelScriptableObjectBase[] UiScriptableObjectsList;

        // 刷新列表
#if UNITY_EDITOR
        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void ResetAllViewObjOverview()
        {
            this.UiScriptableObjectsList = AssetDatabase.FindAssets("t:PanelScriptableObjectBase")
                .Select(guid => AssetDatabase.LoadAssetAtPath<PanelScriptableObjectBase>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
#endif
        
        
        public PanelScriptableObjectBase GetUiViewSO(string viewName)
        {
            foreach (PanelScriptableObjectBase objectBase in UiScriptableObjectsList)
            {
                if (objectBase.PanelObj.name.Equals(viewName))
                {
                    return objectBase;
                }
            }

            Debug.LogError("has not ViewSO");
            return null;
        }
    }
}