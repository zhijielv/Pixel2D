using System;
using System.Collections.Generic;
using Framework.Scripts.Manager;
using Framework.Scripts.UI.Base;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Framework.Scripts.UI.ScriptableObjects
{
    public class PanelScriptableObjectBase : SerializedScriptableObject
    {
        [ReadOnly] public List<string> widgetList = new List<string>();
        [ReadOnly] public GameObject PanelObj;

        #region Reset RegistPanelObj

        public void ResetWidgets()
        {
#if UNITY_EDITOR
            GlobalConfig<UiScriptableObjectsManager>.Instance.ResetAllViewObjOverview();
#endif
            RegistWidgets(PanelObj.transform);
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
                    case UIConfig.Panel:
                        Constants.AddOrGetComponent(child.gameObject, typeof(UiWidgetBase));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                widgetList.Add(child.name);
            }
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

            if (Enum.TryParse(lastName, out UIConfig uiEnum) && !Enum.TryParse(lastName, true, out IgnoreUI ignoreUI))
            {
                uiType = uiEnum;
                return true;
            }

            uiType = null;
            return false;
        }

        #endregion


        // private void OnValidate()
        // {
        //     widgetList?.Clear();
        //     ResetWidgets();
        // }
    }
}