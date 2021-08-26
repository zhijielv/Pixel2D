using System;
using System.Collections.Generic;
using System.Linq;
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
        // [ShowInInspector, ReadOnly] public Dictionary<string, ViewBase> uiList = new Dictionary<string, ViewBase>();
        [ShowInInspector, ReadOnly] public List<ViewBase> uiList = new List<ViewBase>();

        #region public

        // 全屏窗口类型关闭
        public void CloseView(ViewBase viewBase)
        {
            if (!uiList.Contains(viewBase))
            {
                Debug.LogError("has not panel : " + viewBase);
                return;
            }

            viewBase.ShowOrHiddenTurn();
            uiList[uiList.Count - 2].ShowOrHiddenTurn();
            // 显示最后一个View
            // uiList.Last().Value.ShowOrHiddenTurn();
        }
        
        // public void RemoveView(string viewName)
        // {
            // CloseView(viewName);
            // uiList.Remove(viewName);
        // }

        public ViewBase GetViewBase(string viewName)
        {
            for (int i = 0; i < uiList.Count; i++)
            {
                if (uiList[i].transform.name.Equals(viewName))
                    return uiList[i];
            }

            return null;
        }

        #region Enum类型获取

        public UiWidgetBase GetWidget(Enum viewName, Enum widgetName = null)
        {
            var obj = GetWidgetObj(viewName, widgetName) as GameObject;
            return obj != null ? obj.GetComponent<UiWidgetBase>() : null;
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
            if (targetWidgetName == null)
                return GetOrCreateViewObj(viewName).GetComponent<T>();
            var widgetName = targetWidgetName.ToString();
            return GetOrCreateViewObj(viewName, widgetName).GetComponent<T>();
        }

        #endregion

        #endregion

        #region private

        // todo 设置加载View的parent，默认是MainCanvas
        private GameObject GetOrCreateViewObj(string viewName, string widgetName = null)
        {
            ViewBase tempView = GetViewBase(viewName);
            // if (uiList.ContainsKey(viewName))
            if (uiList.Contains(tempView))
            {
                tempView.ShowOrHiddenTurn();
                return tempView.GetWidget(widgetName) as GameObject;
            }

            Debug.Log("Create View : " + viewName);
            GameObject tmpView = AddressableManager.Instance.Instantiate(viewName, Common.MainCanvas.transform);
            tmpView.RemoveClone();
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
            string viewName = widgetObj.name;
            ViewBase tempView = GetViewBase(viewName);
            if (!uiList.Contains(tempView))
            {
                Type type = Type.GetType(Constants.Constants.UiNameSpace + viewName);
                if (type == null)
                {
                    Debug.LogError($"{widgetObj.name} is not Generate");
                    return;
                }

                ViewBase viewBase = widgetObj.GetComponentOrAdd(type) as ViewBase;
                // uiList.Add(viewName, viewBase);
                uiList.Add(viewBase);
            }
            else
            {
                Debug.LogError("has same widget : " + widgetObj.name);
            }
        }

        #endregion
    }
}