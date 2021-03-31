using System;
using System.Collections.Generic;
using Framework;
using Framework.Manager;
using Framework.Scripts.UI.Base;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UI.UiEnumConstant;
using UnityEngine;

namespace UI.ScriptableObjects
{
    public class PanelScriptableObjectBase : SerializedScriptableObject
    {
        [ReadOnly]
        public List<string> widgetList = new List<string>();
        [ReadOnly]
        public GameObject PanelObj;
        
        #region Reset RegistPanelObj
        public void ResetWidgets()
        {
            GlobalConfig<UiScriptableObjectsManager>.Instance.ResetAllViewObjOverview();
            RegistWidgets(PanelObj.transform);
        }

        private void RegistWidgets(Transform obj)
        {
            Transform[] children = obj.GetComponentsInChildren<Transform>();
            
            foreach (Transform child in children)
            {
                if(!CheckName(child.name, out UiEnum uiType)) continue;
                switch (uiType)
                {
                    case UiEnum.View : break;
                    case UiEnum.Default:
                        break;
                    case UiEnum.Panel:
                    case UiEnum.Text:
                    case UiEnum.Button:
                    case UiEnum.Image:
                        Constants.AddOrGetComponent(child.gameObject, typeof(UiWidgetBase));
                        break;
                    case UiEnum.MaxNum:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                widgetList.Add(child.name);
            }
        }

        private bool CheckName(string objName, out UiEnum uiType)
        {
            string[] nameStrings = objName.Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries);
            if (nameStrings.Length <= 1)
            {
                uiType = UiEnum.Default;
                return false;
            }
            string lastName = nameStrings[nameStrings.Length - 1];
            if (widgetList.Contains(lastName))
            {
                Debug.LogError("has same widget Name : " + objName);
                uiType = UiEnum.Default;
                return false;
            }
            if (Enum.TryParse(lastName, out UiEnum uiEnum))
            {
                uiType = uiEnum;
                return true;
            }

            uiType = UiEnum.Default;
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
