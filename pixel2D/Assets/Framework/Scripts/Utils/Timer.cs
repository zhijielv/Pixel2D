using System;
using Framework.Scripts.Manager;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Scripts.Utils
{
    public delegate void TimerDelegate();
    public class Timer
    {
        
        public float LifeTime;
        public float Duration
        {
            get => _duration;
            set
            {
                _duration = value;
            }
        }

        public TimerDelegate InitDelegate;
        public TimerDelegate UpdateDelegate;
        public TimerDelegate CallBackDelegate;
        
        private float _duration;
        private bool _isPause;

        public Timer()
        {
            Duration = 0f;
            _isPause = false;
        }

        public void Init()
        {
            LifeTime = _duration;
            InitDelegate?.Invoke();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isPause) return;
            LifeTime -= deltaTime;
            if (LifeTime <= 0)
            {
                CallBackDelegate?.Invoke();
                TimerManager.Instance.RemoveTimer(this);
            }
            else
                UpdateDelegate?.Invoke();
        }

        public void SetTimerTrick(bool boolean)
        {
            _isPause = boolean;
        }

        public void ChangeState()
        {
            _isPause = !_isPause;
        }
    }
}