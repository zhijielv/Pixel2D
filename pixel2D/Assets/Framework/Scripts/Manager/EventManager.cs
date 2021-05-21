using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Scripts.Singleton;
using UnityEngine;
using EventType = Framework.Scripts.Constants.EventType;

namespace Framework.Scripts.Manager
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventManager : ManagerSingleton<EventManager>
    {
        /// <summary>
        /// 全局事件监听
        /// </summary>
        private static readonly Dictionary<EventType, DelegateEvent> GlobalEventTable =
            new Dictionary<EventType, DelegateEvent>();

        /// <summary>
        /// 局部事件监听
        /// </summary>
        private static readonly Dictionary<object, Dictionary<EventType, DelegateEvent>> EventTable =
            new Dictionary<object, Dictionary<EventType, DelegateEvent>>();

        #region 全局事件方法

        private DelegateEvent GetDelegateEvent(EventType eventType)
        {
            return GlobalEventTable.TryGetValue(eventType, out var getDelegateEvent) ? getDelegateEvent : null;
        }

        // 检查事件删除
        private void CheckForEventRemoval(EventType eventType, DelegateEvent delegateEvent)
        {
            if (delegateEvent.GetEventHandler().GetInvocationList().Length == 0)
            {
                GlobalEventTable.Remove(eventType);
            }
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventHandler">监听函数</param>
        public void AddEventListener(EventType type, EventHandler eventHandler)
        {
            DelegateEvent delegateEvent;
            if (GlobalEventTable.TryGetValue(type, out delegateEvent))
            {
#if UNITY_EDITOR
                EventHandler[] delegates = (EventHandler[]) delegateEvent.GetEventHandler().GetInvocationList();
                for (int i = 0; i < delegates.Length; ++i)
                {
                    if (delegates[i] == eventHandler)
                    {
                        Debug.LogWarning("Warning: the function \"" + eventHandler.Method.ToString() +
                                         "\" is trying to subscribe to the " + type + " more than once." +
                                         eventHandler.Target);
                        break;
                    }
                }
#endif
                delegateEvent.AddListener(eventHandler);
            }
            else
            {
                delegateEvent = ObjectManager.Get<DelegateEvent>();
                delegateEvent.AddListener(eventHandler);
                GlobalEventTable.Add(type, delegateEvent);
            }
        }
        
        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventHandler"></param>
        public void RemoveEventListener(EventType eventType, EventHandler eventHandler)
        {
            DelegateEvent delegateEventBase = GetDelegateEvent(eventType);
            if (delegateEventBase != null)
            {
                EventHandler[] eventHandlers =
                    delegateEventBase.GetEventHandler().GetInvocationList() as EventHandler[];
                System.Diagnostics.Debug.Assert(eventHandlers != null, nameof(eventHandlers) + " != null");
                if (eventHandlers.Contains(eventHandler))
                {
                    delegateEventBase.RemoveListener(eventHandler);
                }

                CheckForEventRemoval(eventType, delegateEventBase);
            }

            Debug.LogError($"{eventType} is not Add");
        }

        /// <summary>
        /// 触发某一类型的事件  并传递数据
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventData">事件的数据(可为null)</param>
        public void DispatchEvent(EventType eventType, EventData eventData = null)
        {
            DelegateEvent delegateEvent = GetDelegateEvent(eventType);
            //创建事件数据
            delegateEvent.Handle(eventData);
        }
        
        public void DispatchEvent<T>(EventType eventType, EventData<T> eventData = null)
        {
            DelegateEvent delegateEvent = GetDelegateEvent(eventType);
            //创建事件数据
            delegateEvent.Handle(eventData);
        }

        #endregion

        #region 局部事件方法

        private DelegateEvent GetDelegateEvent(object obj, EventType eventType)
        {
            Dictionary<EventType, DelegateEvent> delegateEvents =
                EventTable.TryGetValue(obj, out var events) ? events : null;
            return delegateEvents != null && delegateEvents.TryGetValue(eventType, out var delegateEvent) ? delegateEvent : null;
        }
        
        // 检查事件删除
        private void CheckForEventRemoval(object obj, EventType eventType, DelegateEvent delegateEvent)
        {
            if (delegateEvent.GetEventHandlers() == null)
            {
                Dictionary<EventType, DelegateEvent> delegateEvents;
                if (EventTable.TryGetValue(obj, out delegateEvents))
                {
                    delegateEvents.Remove(eventType);
                    if(delegateEvents.Count == 0)
                        EventTable.Remove(obj);
                }
                ObjectManager.Return(delegateEvent);
            }
        }

        public void AddEventListener(object obj, EventType eventType, EventHandler eventHandler)
        {
            Dictionary<EventType, DelegateEvent> delegateEvents;
            DelegateEvent delegateEvent;
            // 无obj，新加
            if (!EventTable.TryGetValue(obj, out delegateEvents))
            {
                delegateEvent = ObjectManager.Get<DelegateEvent>();
                delegateEvent.AddListener(eventHandler);
                delegateEvents = new Dictionary<EventType, DelegateEvent> {{eventType, delegateEvent}};
                EventTable.Add(obj, delegateEvents);
            }
            // 有obj
            else
            {
                // 无eventType
                if (!delegateEvents.TryGetValue(eventType, out delegateEvent))
                {
                    delegateEvent = ObjectManager.Get<DelegateEvent>();
                    delegateEvent.AddListener(eventHandler);
                    EventTable[obj].Add(eventType, delegateEvent);
                }
                // 有eventType
                else
                {
#if UNITY_EDITOR
                    Delegate[] delegates = delegateEvent.GetEventHandlers();
                    for (int i = 0; i < delegates.Length; ++i)
                    {
                        if (delegates[i].Method == eventHandler.Method)
                        {
                            Debug.LogWarning("Warning: the function \"" + eventHandler.Method.ToString() +
                                             "\" is trying to subscribe to the " + eventType + " more than once." +
                                             eventHandler.Target);
                            break;
                        }
                    }
#endif
                    // 无eventHandler
                    delegateEvent.AddListener(eventHandler);
                }
                    
            }
        }
        
        public void RemoveEventListener(object obj, EventType eventType, EventHandler eventHandler)
        {
            var delegateEvent = GetDelegateEvent(obj, eventType);
            if (delegateEvent == null) return;
            Delegate[] eventHandlers = delegateEvent.GetEventHandlers();
            foreach (var handler in eventHandlers)
            {
                EventHandler tmpEventHandler = (EventHandler) handler;
                if (!eventHandler.Equals(tmpEventHandler)) continue;
                delegateEvent.RemoveListener(tmpEventHandler);
                break;
            }
            CheckForEventRemoval(obj, eventType, delegateEvent);
        }
        
        // public void RemoveEventListener(object obj, EventType eventType, EventHandler eventHandler)
        // {
        //     
        //     DelegateEvent delegateEvent = GetDelegateEvent(obj, eventType);
        //     if (delegateEvent == null) return;
        //     Delegate[] eventHandlers = delegateEvent.GetEventHandlers();
        //     System.Diagnostics.Debug.Assert(eventHandlers != null, nameof(eventHandlers) + " != null");
        //     foreach (Delegate handler in eventHandlers)
        //     {
        //         if (!eventHandler.Equals(handler.Method)) continue;
        //         
        //         handler -= eventHandlerMethod;
        //         break;
        //     }
        //     CheckForEventRemoval(obj, eventType, delegateEvent);
        // }

        public void RemoveEventListener(object obj, EventType eventType)
        {
            var delegateEvent = GetDelegateEvent(obj, eventType);
            if (delegateEvent == null) return;
            Dictionary<EventType, DelegateEvent> delegateEvents =
                EventTable.TryGetValue(obj, out var events) ? events : null;
            System.Diagnostics.Debug.Assert(delegateEvents != null, nameof(delegateEvents) + " != null");
            delegateEvents.Remove(eventType);
        }

        /// <summary>
        /// 触发某一类型的事件  并传递数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventType">事件类型</param>
        /// <param name="data">事件的数据(可为null)</param>
        public void DispatchEvent(object obj, EventType eventType, EventData eventData = null)
        {
            DelegateEvent list = GetDelegateEvent(obj, eventType);
            //创建事件数据
            list?.Handle(eventData);
        }

        #endregion
    }

    #region DelegateEvent

    /// <summary>
    /// 事件抽象类
    /// </summary>
    public class DelegateEvent
    {
        /// <summary>
        /// 定义基于委托函数的事件
        /// </summary>
        private event EventHandler EventHandle;

        /// <summary>
        /// 触发监听事件
        /// </summary>
        /// <param name="data">事件传过来的参数</param>
        public void Handle(EventDataBase data)
        {
            EventHandle?.Invoke(this, data);
        }

        /// <summary>
        /// 获取委托
        /// </summary>
        /// <returns></returns>
        public EventHandler GetEventHandler()
        {
            return EventHandle;
        }

        public Delegate[] GetEventHandlers()
        {
            Delegate[] invocationList = EventHandle?.GetInvocationList();
            return invocationList;
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
    public abstract class EventDataBase : EventArgs
    {
        /// <summary>
        /// 事件传递的数据
        /// </summary>
        public object Data;
    }

    public class EventData : EventDataBase
    {
    }

    public class EventData<T> : EventDataBase
    {
        public T Value;

        public EventData(T value)
        {
            this.Value = value;
        }
    }

    #endregion
}