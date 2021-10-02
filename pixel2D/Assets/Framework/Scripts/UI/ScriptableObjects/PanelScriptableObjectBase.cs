using System;
using System.Collections.Generic;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;    
#endif
using UnityEngine;
// 用来保存数据
namespace Framework.Scripts.UI.ScriptableObjects
{
    public class PanelScriptableObjectBase : SerializedScriptableObject
    {
        [ReadOnly] public List<string> widgetList = new List<string>();
        [ReadOnly] public GameObject panelObj;

        public Dictionary<string, object> SerializeObj = new Dictionary<string, object>();
        #region Reset RegistPanelObj
#if UNITY_EDITOR
        [Button("赋值Obj")]
        public void ResetWidgets()
        {
            if (panelObj == null)
            {
                GlobalConfig<UiScriptableObjectsManager>.Instance.ResetAllViewPrefab();
                GlobalConfig<UiScriptableObjectsManager>.Instance.ResetAllViewSo();
            }

            RegistWidgets(panelObj.transform);
        }

        private void RegistWidgets(Transform obj)
        {
            Transform[] children = obj.GetComponentsInChildren<Transform>();

            foreach (Transform child in children)
            {
                if (!CheckName(child.name, out UIConfig? uiType) || widgetList.Contains(child.name)) continue;
                widgetList.Add(child.name);
            }
            AssetDatabase.Refresh();
        }

        private bool CheckName(string objName, out UIConfig? uiType)
        {
            string[] nameStrings = objName.Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries);
            if (nameStrings.Length <= 1)
            {
                uiType = null;
                return false;
            }

            string lastName = nameStrings[nameStrings.Length - 1];
            if (widgetList.Contains(lastName))
            {
                Debug.LogError("has same widget Name : " + objName);
                uiType = null;
                return false;
            }

            if (Enum.TryParse(lastName, out UIConfig uiEnum) && !Enum.TryParse(lastName, true, out IgnoreUI ignoreUi))
            {
                uiType = uiEnum;
                return true;
            }

            uiType = null;
            return false;
        }
#endif
        #endregion
    }
}