using System;
using System.Collections.Generic;
using Framework.Scripts.Constants;
using Framework.Scripts.Singleton;

namespace Framework.Scripts.Manager
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventManager : ManagerSingleton<EventManager>
    {
        /// <summary>
        /// 事件监听池
        /// </summary>
        private static readonly Dictionary<EventConstants, DelegateEvent> EventConstantsListeners =
            new Dictionary<EventConstants, DelegateEvent>();

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="listenerFunc">监听函数</param>
        public void AddEventListener(EventConstants type, DelegateEvent.EventHandler listenerFunc)
        {
            DelegateEvent delegateEvent;
            if (EventConstantsListeners.ContainsKey(type))
            {
                delegateEvent = EventConstantsListeners[type];
            }
            else
            {
                delegateEvent = new DelegateEvent();
                EventConstantsListeners[type] = delegateEvent;
            }

            delegateEvent.AddListener(listenerFunc);
        }

        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="listenerFunc">监听函数</param>
        public void RemoveEventListener(EventConstants type, DelegateEvent.EventHandler listenerFunc)
        {
            if (listenerFunc == null)
            {
                return;
            }

            if (!EventConstantsListeners.ContainsKey(type))
            {
                return;
            }

            DelegateEvent delegateEvent = EventConstantsListeners[type];
            delegateEvent.RemoveListener(listenerFunc);
        }

        /// <summary>
        /// 触发某一类型的事件  并传递数据
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="data">事件的数据(可为null)</param>
        public void DispatchEvent(EventConstants type, object data)
        {
            if (!EventConstantsListeners.ContainsKey(type))
            {
                return;
            }

            //创建事件数据
            EventData eventData = new EventData {Type = type, Data = data};

            DelegateEvent delegateEvent = EventConstantsListeners[type];
            delegateEvent.Handle(eventData);
        }
    }

    /// <summary>
    /// 事件类
    /// </summary>
    public class DelegateEvent
    {
        /// <summary>
        /// 定义委托函数
        /// </summary>
        /// <param name="data"></param>
        public delegate void EventHandler(EventData data);

        /// <summary>
        /// 定义基于委托函数的事件
        /// </summary>
        public event EventHandler EventHandle;

        /// <summary>
        /// 触发监听事件
        /// </summary>
        /// <param name="data"></param>
        public void Handle(EventData data)
        {
            EventHandle?.Invoke(data);
        }

        /// <summary>
        /// 删除监听函数
        /// </summary>
        /// <param name="removeHandle"></param>
        public void RemoveListener(EventHandler removeHandle)
        {
            if (EventHandle != null)
                EventHandle -= removeHandle;
        }

        /// <summary>
        /// 添加监听函数
        /// </summary>
        /// <param name="addHandle"></param>
        public void AddListener(EventHandler addHandle)
        {
            EventHandle += addHandle;
        }
    }

    /// <summary>
    /// 事件数据
    /// </summary>
    public class EventData : EventArgs
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public EventConstants Type;

        /// <summary>
        /// 事件传递的数据
        /// </summary>
        public object Data;
    }
}