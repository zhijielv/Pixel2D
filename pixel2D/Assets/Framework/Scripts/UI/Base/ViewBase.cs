/*
** Created by fengling
** DateTime:    2021-03-21 14:09:20
** Description: UI界面基类
*/

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Scripts.UI.Base
{
    public class ViewBase : UiWidgetBase
    {
        [ShowInInspector, ReadOnly]
        public Dictionary<string, UiWidgetBase> widgetDic;

        private GameObject GetWidget(string widgetName)
        {
            if (widgetDic.ContainsKey(widgetName))
            {
                return widgetDic[widgetName].gameObject;
            }
            else
            {
                Debug.LogError(gameObject.name + " has not widget : " + widgetName);
                return null;
            }
        }

        public GameObject GetWidget(Enum @enum)
        {
            return GetWidget(@enum.ToString());
        }

        public void Awake()
        {
            widgetDic = new Dictionary<string, UiWidgetBase>();
            foreach (UiWidgetBase uiWidgetBase in transform.GetComponentsInChildren<UiWidgetBase>())
            {
                widgetDic.Add(uiWidgetBase.name, uiWidgetBase);
            }
        }
    }
}