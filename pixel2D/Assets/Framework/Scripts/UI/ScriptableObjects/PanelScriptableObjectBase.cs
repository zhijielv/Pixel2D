using System;
using System.Collections.Generic;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using Framework.Scripts.UI.Base;
using Framework.Scripts.UI.CustomUI;
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

        #region Reset RegistPanelObj

        public void ResetWidgets()
        {
#if UNITY_EDITOR
            GlobalConfig<UiScriptableObjectsManager>.Instance.ResetAllViewSO();
#endif
            RegistWidgets(panelObj.transform);
        }

        private void RegistWidgets(Transform obj)
        {
            Transform[] children = obj.GetComponentsInChildren<Transform>();

            foreach (Transform child in children)
            {
                if (!CheckName(child.name, out UIConfig? uiType)) continue;
                switch (uiType)
                {
                    case UIConfig.Text:
                    case UIConfig.Button:
                    case UIConfig.Image:
                    case UIConfig.InputField:
                    case UIConfig.TouchController:
                        Constants.Constants.AddOrGetComponent(child.gameObject, typeof(UiWidgetBase));
                        break;
                    case UIConfig.CustomPanel:
                        Constants.Constants.AddOrGetComponent(child.gameObject, typeof(CustomPanel));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                widgetList.Add(child.name);
            }
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
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

        #endregion
    }
}