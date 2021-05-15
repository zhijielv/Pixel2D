﻿/*
** Created by fengling
** DateTime:    2021-03-21 14:09:20
** Description: UI界面基类
** TODO 添加生命周期，隐藏N秒后自动销毁
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using Rewired;
using Sirenix.OdinInspector;
using SRF;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.Scripts.UI.Base
{
    public class ViewBase : UiWidgetBase
    {
        readonly struct EventItem
        {
            public readonly EventConstants EventType;
            public readonly DelegateEvent.EventHandler ListenerFunc;

            public EventItem(EventConstants type, DelegateEvent.EventHandler listenerFunc)
            {
                EventType = type;
                ListenerFunc = listenerFunc;
            }
        }

        #region EventHandler

        private readonly List<EventItem> _eventItems = new List<EventItem>();
        private readonly List<Action<InputActionEventData>> _viewEvents =
            new List<Action<InputActionEventData>>();

        protected void AddEventListener(EventConstants type, DelegateEvent.EventHandler listenerFunc)
        {
            EventManager.Instance.AddEventListener(type, listenerFunc);
            _eventItems.Add(new EventItem(type, listenerFunc));
        }

        protected void AddInputEventDelegate(Action<InputActionEventData> callback,
            UpdateLoopType updateLoop,
            InputActionEventType eventType,
            string actionName)
        {
            _viewEvents.Add(callback);
            RewiredInputEventManager.Instance.AddEvent(callback, updateLoop, eventType, actionName);
        }

        protected void Disable()
        {
            foreach (EventItem eventItem in _eventItems)
                EventManager.Instance.RemoveEventListener(eventItem.EventType, eventItem.ListenerFunc);

            foreach (Action<InputActionEventData> action in _viewEvents)
            {
                RewiredInputEventManager.Instance.RemoveEvent(action);
            }
            _viewEvents.Clear();
        }

        #endregion

        #region public

        internal virtual object GetWidget(string widgetName)
        {
            Type viewType = GetType();
            // 反射赋值
            FieldInfo[] fieldInfos = viewType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.Name != widgetName) continue;
                var obj = fieldInfo.GetValue(gameObject.GetComponent<ViewBase>());
                var o = Convert.ChangeType(obj, fieldInfo.FieldType);
                return o;
            }

            return null;
        }

        public void ChangePanel<T>() where T : ViewBase
        {
            if (typeof(T) == GetType()) return;
            ShowWithHiddenTurn();
            ViewBase targetView = (ViewBase) UiManager.Instance.GetWidget<T>();
            targetView.ShowWithHiddenTurn();
        }

        public void AddButtonClickEvent(Button button, UnityAction callback)
        {
            button.GetComponent<UiWidgetBase>().AddButtonClickEvent(callback);
        }

        public void AddInputFieldValueChangedEvent(string widgetName, UnityAction<string> callback)
        {
            AddInputFieldValueChangedEvent(callback);
        }

        public void AddInputFieldEndEditEvent(string widgetName, UnityAction<string> callback)
        {
            AddInputFieldEndEditEvent(callback);
        }

        public void TextWrite(string widgetName, string value)
        {
            TextWrite(value);
        }

        public void TextClear(string widgetName)
        {
            TextClear();
        }

        public void AddToggleValueChangedEvent(string widgetName, UnityAction<bool> callback)
        {
            AddToggleValueChangedEvent(callback);
        }

        public void AddSliderValueChangedEvent(string widgetName, UnityAction<float> callback)
        {
            AddSliderValueChangedEvent(callback);
        }

        public void AddDropdownValueChangedEvent(string widgetName, UnityAction<int> callback)
        {
            AddDropdownValueChangedEvent(callback);
        }

        public void AddScrollValueChangedEvent(string widgetName, UnityAction<Vector2> callback)
        {
            AddScrollValueChangedEvent(callback);
        }


        public void AddClickEvent(string widgetName, UnityAction<BaseEventData> callback)
        {
            AddInterFaceEvent(EventTriggerType.PointerClick, callback);
        }

        public void AddClickEvent(string subParentName, string subName, UnityAction<BaseEventData> callback)
        {
            AddInterFaceEvent(EventTriggerType.PointerClick, callback);
        }

        public void AddBeginDragEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.BeginDrag, callBack);
        }

        public void AddBeginDragEvent(string subParentName, string subName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.BeginDrag, callBack);
        }

        public void AddOnDragEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.Drag, callBack);
        }

        public void AddOnDragEvent(string subParentName, string subName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.Drag, callBack);
        }

        public void AddEndDragEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.EndDrag, callBack);
        }

        public void AddEndDragEvent(string subParentName, string subName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.EndDrag, callBack);
        }

        public void AddPointerEnterEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.PointerEnter, callBack);
        }

        public void AddPointerEnterEvent(string subPaentName, string subName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.PointerEnter, callBack);
        }

        public void AddPointerExitEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.PointerExit, callBack);
        }

        public void AddPointerExitEvent(string subParentName, string subName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.PointerExit, callBack);
        }

        #endregion

        #region virtual

        #endregion
    }
}