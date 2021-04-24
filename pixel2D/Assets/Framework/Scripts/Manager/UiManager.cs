using System;
using System.Collections.Generic;
using Framework.Scripts.Constants;
using Framework.Scripts.Singleton;
using Framework.Scripts.UI.Base;
using Framework.Scripts.UI.ScriptableObjects;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.Scripts.Manager
{
    public class UiManager : ManagerSingleton<UiManager>
    {
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
            var obj = GetWidgetObj(viewName, widgetName) as Component;
            return obj.GetComponent<UiWidgetBase>();
        }

        public T GetWidgetComponent<T>(Enum viewName, Enum widgetName)
        {
            return (T) GetWidgetObj(viewName, widgetName);
        }

        public object GetWidgetComponent(Enum viewName, Enum widgetName)
        {
            return GetWidgetObj(viewName, widgetName);
        }

        public object GetWidgetObj(Enum targetViewName, Enum targetWidgetName)
        {
            string viewName = targetViewName.ToString();
            string widgetName = targetWidgetName.ToString();
            // Debug.Log($"{viewName}    {widgetName}");
            return GetOrCreateViewObj(viewName, widgetName);
        }

        #endregion

        #region 泛型类型获取

        public object GetWidget<T>(Enum viewName = null) where T : ViewBase
        {
            return GetWidgetObj<T>(viewName);
        }

        // todo UICanvas Parent
        public object GetWidgetObj<T>(Enum targetWidgetName = null) where T : ViewBase
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

        private object GetOrCreateViewObj(string viewName, string widgetName)
        {
            if (widgetName.IsNullOrWhitespace()) widgetName = viewName;
            if (uiList.ContainsKey(viewName))
            {
                return uiList[viewName].GetWidget(widgetName);
            }

            Debug.Log("Create View : " + viewName);
            GameObject viewPanelObj =
                GlobalConfig<UiScriptableObjectsManager>.Instance.GetUiViewObj(viewName);
            // todo 设置加载View的parent，默认是MainCanvas
            GameObject tmpView = Instantiate(viewPanelObj, Common.MainCanvas.transform);
            RegistView(tmpView);
            GameObject widgetObj = (GameObject) tmpView.GetComponent<ViewBase>().GetWidget(widgetName);
            if (widgetObj != null) return widgetObj;
            Debug.Log(viewName + " has not widget : " + widgetName);
            return null;
        }

        private void RegistView(GameObject widgetObj)
        {
            string viewName = Constants.Constants.ReplaceString(widgetObj.name, "(Clone)", "");
            if (!uiList.ContainsKey(viewName))
            {
                Type type = Type.GetType(Constants.Constants.UiNameSpace + viewName);
                if (type == null)
                {
                    Debug.LogError($"{widgetObj.name} is not Generate");
                    return;
                }

                ViewBase viewBase = (ViewBase) Constants.Constants.AddOrGetComponent(widgetObj, type);
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