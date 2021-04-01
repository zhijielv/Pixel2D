using System;
using System.Collections.Generic;
using Framework;
using Framework.Scripts.Manager;
using Framework.Scripts.UI.Base;
using Framework.Scripts.UI.ScriptableObjects;
using Framework.Singleton;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Manager
{
    public class UiManager : ManagerSingleton<UiManager>
    {
        public Canvas mainCanvas;
        [ShowInInspector, ReadOnly] public Dictionary<string, ViewBase> uiList = new Dictionary<string, ViewBase>();

        #region public
        public void RemoveWidget(string panelName)
        {
            if (!uiList.ContainsKey(panelName))
            {
                Debug.LogError("has not panel : " + panelName);
                return;
            }

            uiList.Remove(panelName);
        }

        #region Enum类型获取
        public UiWidgetBase GetWidget(Enum viewName, Enum widgetName)
        {
            return GetWidgetObj(viewName, widgetName).GetComponent<UiWidgetBase>();
        }

        public GameObject GetWidgetObj(Enum targetViewName, Enum targetWidgetName)
        {
            string viewName = targetViewName.ToString();
            string widgetName = targetWidgetName.ToString();
            return GetOrCreateViewObj(viewName, widgetName);
        }
        #endregion
        
        #region 泛型类型获取
        public UiWidgetBase GetWidget<T>(Enum viewName = null) where T : ViewBase
        {
            return GetWidgetObj<T>(viewName).GetComponent<UiWidgetBase>();
        }

        // todo UICanvas Parent
        public GameObject GetWidgetObj<T>(Enum targetWidgetName = null) where T : ViewBase
        {
            string viewName = typeof(T).Name;
            string widgetName = viewName;
            if (targetWidgetName != null)
                widgetName = targetWidgetName.ToString();
            return GetOrCreateViewObj(viewName, widgetName);
        }
        #endregion
        
        #endregion

        #region private

        private GameObject GetOrCreateViewObj(string viewName, string widgetName)
        {
            if (widgetName.IsNullOrWhitespace()) widgetName = viewName;
            if (uiList.ContainsKey(viewName))
            {
                return uiList[viewName].GetWidget(widgetName).gameObject;
            }

            Debug.Log("Create View : " + viewName);
            PanelScriptableObjectBase tmpSoBase =
                GlobalConfig<UiScriptableObjectsManager>.Instance.GetUiViewSO(viewName);
            if (tmpSoBase.widgetList.Contains(widgetName))
            {
                // todo 设置parent
                GameObject tmpView = Instantiate(tmpSoBase.PanelObj, mainCanvas.transform);
                RegistView(tmpView);
                return tmpView;
            }

            Debug.Log(viewName + " has not widget : " + widgetName);
            return null;
        }
        private void RegistView(GameObject widgetObj)
        {
            string viewName = Constants.ReplaceString(widgetObj.name, "(Clone)", "");
            Debug.Log(viewName);
            if (!uiList.ContainsKey(viewName))
            {
                Type type = Type.GetType(Constants.UiNameSpace + viewName);
                if (type == null)
                {
                    Debug.LogError($"{widgetObj.name} is not Generate");
                    return;
                }
                ViewBase viewBase = (ViewBase) Constants.AddOrGetComponent(widgetObj, type);
                uiList.Add(viewName, viewBase);
            }
            else
            {
                Debug.LogError("has same widget : " + widgetObj.name);
            }
        }

        #endregion
    }
}