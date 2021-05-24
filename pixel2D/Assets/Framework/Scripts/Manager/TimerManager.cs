using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Singleton;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework.Scripts.Manager
{
    public class TimerManager : ManagerSingleton<TimerManager>
    {
        [ShowInInspector, ReadOnly, LabelText("计时器")]
        private readonly List<Timer> _timers = new List<Timer>();

        [LabelText("Update事件")] [ShowInInspector]
        private Dictionary<ScheduledEventBase, EventDataBase> m_ActiveUpdateEvents;

        private List<ScheduledEventBase> m_RemoveUpdateEvents;

        [LabelText("FixedUpdate事件")] [ShowInInspector]
        private Dictionary<ScheduledEventBase, EventDataBase> m_ActiveFixedUpdateEvents;

        private List<ScheduledEventBase> m_RemoveFixedUpdateEvents;

        public override Task Init()
        {
            m_ActiveUpdateEvents = new Dictionary<ScheduledEventBase, EventDataBase>();
            m_RemoveUpdateEvents = new List<ScheduledEventBase>();
            m_ActiveFixedUpdateEvents = new Dictionary<ScheduledEventBase, EventDataBase>();
            m_RemoveFixedUpdateEvents = new List<ScheduledEventBase>();
            return base.Init();
        }

        private void Update()
        {
            // Schedule
            foreach (var updateEvent in m_ActiveUpdateEvents)
            {
                Invoke(updateEvent.Key, updateEvent.Value);
            }

            CheckCheckForScheduledRemoval(ScheduledEventBase.InvokeLocation.Update);

            // Timer
            if (_timers.Count == 0)
            {
                return;
            }

            foreach (var t in _timers)
            {
                t.OnUpdate(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            // Schedule
            foreach (var updateEvent in m_ActiveFixedUpdateEvents)
            {
                Invoke(updateEvent.Key, updateEvent.Value);
            }

            CheckCheckForScheduledRemoval(ScheduledEventBase.InvokeLocation.FixedUpdate);
        }

        #region Timer

        public void AddTimer(Timer timer)
        {
            if (_timers.Contains(timer))
            {
                timer.LifeTime += timer.Duration;
            }
            else
            {
                _timers.Add(timer);
                timer.Init();
            }
        }

        public void RemoveTimer(Timer timer)
        {
            Assert.IsNotNull(timer, "timer != null");
            if (_timers.Contains(timer))
                _timers.Remove(timer);
        }

        public void ClearTimer(Timer timer)
        {
            if (!_timers.Contains(timer) || timer == null) return;
            timer.LifeTime = 0;
            timer.UpdateDelegate?.Invoke();
            timer.CallBackDelegate?.Invoke();
            RemoveTimer(timer);
        }

        #endregion

        #region Schedule

        /// <summary>
        /// Schedule a new event to occur after the specified delay within the Update loop.
        /// </summary>
        /// <param name="delay">The time to wait before triggering the event.</param>
        /// <param name="action">The event to occur.</param>
        /// <param name="eventData"></param>
        /// <returns>The ScheduledEventBase instance, useful if the event should be cancelled.</returns>
        public ScheduledEventBase Schedule(float delay, EventHandler action, EventDataBase eventData)
        {
            return AddEventInternal(delay, action, eventData, ScheduledEventBase.InvokeLocation.Update);
        }

        /// <summary>
        /// Schedule a new event to occur after the specified delay within the FixedUpdate loop.
        /// </summary>
        /// <param name="delay">The time to wait before triggering the event.</param>
        /// <param name="action">The event to occur.</param>
        /// <param name="eventData"></param>
        /// <returns>The ScheduledEventBase instance, useful if the event should be cancelled.</returns>
        public ScheduledEventBase ScheduleFixed(float delay, EventHandler action, EventDataBase eventData)
        {
            return AddEventInternal(delay, action, eventData, ScheduledEventBase.InvokeLocation.FixedUpdate);
        }

        public void Cancel(ScheduledEventBase scheduledEvent)
        {
            if (scheduledEvent != null && scheduledEvent.Active)
            {
                RemoveActiveEvent(scheduledEvent);
            }
        }

        private ScheduledEventBase AddEventInternal(float delay, EventHandler action, EventDataBase eventData,
            ScheduledEventBase.InvokeLocation invokeLocation)
        {
            if (Math.Abs(delay) < Common.Tolerance)
            {
                action.Invoke(action.Target, eventData);
                return null;
            }

            var scheduledEvent = ObjectManager.Get<ScheduledEvent>();
            // A delay of -1 indicates that the event reoccurs forever and must manually be cancelled.
            scheduledEvent.Initialize((delay == -1 ? -1 : Time.time + delay), invokeLocation, action);
            AddScheduledEvent(scheduledEvent, eventData);
            return scheduledEvent;
        }

        private void Invoke(ScheduledEventBase scheduledEvent, EventDataBase updateEventValue)
        {
            if (scheduledEvent.EndTime == -1 || !scheduledEvent.Active) return;
            if (scheduledEvent.EndTime > Time.time) return;
            scheduledEvent.Invoke(updateEventValue);
            RemoveActiveEvent(scheduledEvent);
        }

        /// <summary>
        /// Adds the scheduled event to the ActiveUpdateEvents or ActiveFixedUpdate events array.
        /// </summary>
        /// <param name="scheduledEvent">The scheduled event to add.</param>
        /// <param name="eventData"></param>
        private void AddScheduledEvent(ScheduledEventBase scheduledEvent, EventDataBase eventData)
        {
            switch (scheduledEvent.Location)
            {
                case ScheduledEventBase.InvokeLocation.Update:
                    m_ActiveUpdateEvents.Add(scheduledEvent, eventData);
                    break;
                case ScheduledEventBase.InvokeLocation.FixedUpdate:
                    m_ActiveFixedUpdateEvents.Add(scheduledEvent, eventData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            scheduledEvent.Active = true;
        }

        private void RemoveActiveEvent(ScheduledEventBase scheduledEvent)
        {
            switch (scheduledEvent.Location)
            {
                case ScheduledEventBase.InvokeLocation.Update:
                    m_RemoveUpdateEvents.Add(scheduledEvent); break;
                case ScheduledEventBase.InvokeLocation.FixedUpdate:
                    m_RemoveFixedUpdateEvents.Add(scheduledEvent); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void CheckCheckForScheduledRemoval(ScheduledEventBase.InvokeLocation invokeLocation)
        {
            switch (invokeLocation)
            {
                case ScheduledEventBase.InvokeLocation.Update:
                    foreach (ScheduledEventBase scheduledEventBase in m_RemoveUpdateEvents)
                    {
                        m_ActiveUpdateEvents.Remove(scheduledEventBase);
                    }

                    m_RemoveUpdateEvents.Clear();

                    break;
                case ScheduledEventBase.InvokeLocation.FixedUpdate:
                    foreach (ScheduledEventBase scheduledEventBase in m_RemoveFixedUpdateEvents)
                    {
                        m_RemoveFixedUpdateEvents.Remove(scheduledEventBase);
                    }

                    m_RemoveFixedUpdateEvents.Clear();

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(invokeLocation), invokeLocation, null);
            }
        }

        #endregion
    }

    [Serializable]
    public abstract class ScheduledEventBase
    {
        /// <summary>
        /// Specifies where the event should be invoked.
        /// </summary>
        public enum InvokeLocation
        {
            Update, // Within MonoBehaviour.Update.
            FixedUpdate // Within MonoBehaviour.FixedUpdate.
        }

        [ShowInInspector, ReadOnly]
        [LabelText("激活")]
        public bool Active { get; set; }

        [ShowInInspector, ReadOnly] [LabelText("结束时间")]
        protected float m_EndTime;

        [ShowInInspector, ReadOnly] [LabelText("执行类型")]
        protected InvokeLocation m_InvokeLocation;

        public float EndTime => m_EndTime;
        public InvokeLocation Location => m_InvokeLocation;

        /// <summary>
        /// Executes the delegate.
        /// </summary>
        public abstract void Invoke(EventDataBase eventData);

#if UNITY_EDITOR
        /// <summary>
        /// Returns the target of the action. Used by the Scheduler inspector.
        /// </summary>
        /// <returns>The value of the target.</returns>
        public abstract object Target { get; }

        /// <summary>
        /// Returns the Method of the action. Used by the Scheduler inspector.
        /// </summary>
        /// <returns>The info of the Method.</returns>
        public abstract System.Reflection.MethodInfo Method { get; }
#endif
    }

    public class ScheduledEvent : ScheduledEventBase
    {
        private DelegateEvent _delegateEvent;

        public void Initialize(float endTime, InvokeLocation invokeLocation, EventHandler eventHandler)
        {
            m_InvokeLocation = invokeLocation;
            m_EndTime = endTime;
            _delegateEvent = ObjectManager.Get<DelegateEvent>();
            _delegateEvent.AddListener(eventHandler);
        }

        public override void Invoke(EventDataBase eventData)
        {
            _delegateEvent.Handle(eventData);
        }

#if UNITY_EDITOR
        public override object Target
        {
            get { return _delegateEvent.GetEventHandler().Target; }
        }

        public override System.Reflection.MethodInfo Method
        {
            get { return _delegateEvent.GetEventHandler().Method; }
        }
#endif
    }
}