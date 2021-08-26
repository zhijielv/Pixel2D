/*
** Created by fengling
** DateTime:    2021-03-21 14:09:20
** Description: UI界面基类
** TODO 添加生命周期，隐藏N秒后自动销毁
** TODO : 参数改为EventHandler
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Framework.Scripts.Manager;
using Rewired;
using EventType = Framework.Scripts.Constants.EventType;

namespace Framework.Scripts.UI.Base
{
    public class ViewBase : UiWidgetBase
    {
        private readonly Dictionary<EventType, DelegateEvent> _viewEvents = new Dictionary<EventType, DelegateEvent>();

        #region EventHandler

        private readonly List<Action<InputActionEventData>> _viewInputEvents =
            new List<Action<InputActionEventData>>();

        protected void AddEventListener(EventType eventType, EventHandler listenerFunc)
        {
            EventManager.Instance.AddEventListener(this, eventType, listenerFunc);
            if (_viewEvents.TryGetValue(eventType, out var delegateEvent))
            {
                delegateEvent.AddListener(listenerFunc);
                return;
            }

            delegateEvent = ObjectManager.Get<DelegateEvent>();
            delegateEvent.AddListener(listenerFunc);
            _viewEvents.Add(eventType, delegateEvent);
        }

        protected void AddInputEventDelegate(Action<InputActionEventData> callback,
            UpdateLoopType updateLoop,
            InputActionEventType eventType,
            string actionName)
        {
            _viewInputEvents.Add(callback);
            RewiredInputEventManager.Instance.AddEvent(callback, updateLoop, eventType, actionName);
        }

        protected void Disable()
        {
            foreach (var eventItem in _viewEvents)
            {
                EventManager.Instance.RemoveEventListener(this, eventItem.Key);
                ObjectManager.Return(eventItem.Value);
            }

            foreach (Action<InputActionEventData> action in _viewInputEvents)
            {
                RewiredInputEventManager.Instance.RemoveEvent(action);
            }

            _viewInputEvents.Clear();
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
            ShowOrHiddenTurn();
            UiManager.Instance.GetWidget<T>();
            // T targetView = UiManager.Instance.GetWidget<T>();
            // targetView.ShowOrHiddenTurn();
        }
        
        public virtual void Close()
        {
            UiManager.Instance.CloseView(this);
        }

        #endregion
    }
}