using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Singleton;
using Framework.Scripts.UI.Base;
using Sirenix.OdinInspector;
using SRF;
using UnityEngine;

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

        public UiWidgetBase GetWidget(Enum viewName, Enum widgetName = null)
        {
            var obj = GetWidgetObj(viewName, widgetName) as GameObject;
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

        public object GetWidgetObj(Enum targetViewName, Enum targetWidgetName = null)
        {
            string viewName = targetViewName.ToString();
            string widgetName = null;
            if (targetWidgetName != null)
            {
                widgetName = targetWidgetName.ToString();
            }
            return GetOrCreateViewObj(viewName, widgetName);
        }

        #endregion

        #region 泛型类型获取

        public T GetWidget<T>(Enum viewName = null) where T : ViewBase
        {
            return GetWidgetObj<T>(viewName);
        }

        // todo UICanvas Parent
        public T GetWidgetObj<T>(Enum targetWidgetName = null) where T : ViewBase
        {
            string viewName = typeof(T).Name;
            if (targetWidgetName == null) return GetOrCreateViewObj(viewName).GetComponent<T>();
            var widgetName = targetWidgetName.ToString();
            return GetOrCreateViewObj(viewName, widgetName).GetComponent<T>();
        }

        #endregion

        #endregion

        #region private

        // todo 设置加载View的parent，默认是MainCanvas
        private GameObject GetOrCreateViewObj(string viewName, string widgetName = null)
        {
            if (uiList.ContainsKey(viewName))
            {
                return uiList[viewName].GetWidget(widgetName) as GameObject;
            }

            Debug.Log("Create View : " + viewName);
            GameObject tmpView = AddressableManager.Instance.Instantiate(viewName, Common.MainCanvas.transform);
            RegistView(tmpView);
            if (widgetName == null)
            {
                return tmpView;
            }

            GameObject widgetObj = (GameObject) tmpView.GetComponent<ViewBase>().GetWidget(widgetName);
            if (widgetObj != null) return widgetObj;
            Debug.LogError(viewName + " has not widget : " + widgetName);
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

                ViewBase viewBase = widgetObj.GetComponentOrAdd(type) as ViewBase;
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