using System.Collections.Generic;
using System.Diagnostics;
using Framework.Scripts.Singleton;
using Framework.Scripts.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework.Scripts.Manager
{
    public class TimerManager : ManagerSingleton<TimerManager>
    {
        private readonly List<Timer> _timers = new List<Timer>();
        private void Update()
        {
            if (_timers.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _timers.Count; i++)
            {
                _timers[i].OnUpdate(Time.deltaTime);
            }
        }

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
            if(_timers.Contains(timer))
                _timers.Remove(timer);
        }
        
        public void ClearTimer(Timer timer)
        {
            if(!_timers.Contains(timer) || timer == null) return;
            timer.LifeTime = 0;
            timer.UpdateDelegate?.Invoke();
            timer.CallBackDelegate?.Invoke();
            RemoveTimer(timer);
        }
    }
}