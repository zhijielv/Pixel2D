using System;
using System.Collections.Generic;
using Framework;
using Framework.Manager;
using Framework.Scripts.UI.Base;
using Framework.Singleton;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UI.ScriptableObjects;
using UnityEngine;

namespace Manager
{
    public class UiManager : ManagerSingleton<UiManager>
    {
        public Canvas mainCanvas;
        [ShowInInspector, ReadOnly] public Dictionary<string, ViewBase> uiList = new Dictionary<string, ViewBase>();

        public void RegistView<T>(GameObject widgetObj) where T : ViewBase
        {
            string panelName = typeof(T).Name;
            if (!uiList.ContainsKey(panelName))
            {
                ViewBase viewBase = (ViewBase) Constants.AddOrGetComponent(widgetObj, typeof(T));
                uiList.Add(panelName, viewBase);
            }
            else
            {
                Debug.LogError("has same widget : " + widgetObj.name);
            }
        }

        public void RemoveWidget(string panelName)
        {
            if (!uiList.ContainsKey(panelName))
            {
                Debug.LogError("has not panel : " + panelName);
                return;
            }

            uiList.Remove(panelName);
        }

        // 加载View
        public GameObject GetView<T>(Enum widgetName) where T : ViewBase
        {
            string panelName = typeof(T).Name;
            if (uiList.ContainsKey(panelName))
            {
                return uiList[panelName].GetWidget(widgetName);
            }

            Debug.Log("Create View : " + panelName);
            PanelScriptableObjectBase tmpSoBase =
                GlobalConfig<UiScriptableObjectsManager>.Instance.GetUiViewSO(panelName);
            if (tmpSoBase.widgetList.Contains(widgetName.ToString()))
            {
                GameObject tmpView = Instantiate(tmpSoBase.PanelObj, mainCanvas.transform);
                RegistView<T>(tmpView);
                return tmpView;
            }

            Debug.Log(panelName + " has not widget : " + widgetName);
            return null;
        }
    }
}