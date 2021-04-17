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
    [GlobalConfig("Art/ScriptableObject/UIScriptableObjectManager")]
    public class UiScriptableObjectsManager : GlobalConfig<UiScriptableObjectsManager>
    {
        [ShowInInspector]
        [ReadOnly]
        [ListDrawerSettings(Expanded = true)]
        public PanelScriptableObjectBase[] UiScriptableObjectsList;
        
        public bool isGenerateCode = false;

        [ReadOnly]
        public Object[] selectViews;

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