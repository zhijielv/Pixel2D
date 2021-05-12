/*
** Created by fengling
** DateTime:    2021-05-12 15:08:27
** Description: 管理Rewired输入事件的注册及卸载
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.Singleton;
using Rewired;
using UnityEngine;

namespace Framework.Scripts.Manager
{
    public class RewiredInputEventManager : ManagerSingleton<RewiredInputEventManager>
    {
        public Player player0 => ReInput.players.GetPlayer(0);

        private List<Action<InputActionEventData>> _inputEventDelegateActions =
            new List<Action<InputActionEventData>>();

        public void AddEvent(Action<InputActionEventData> callback,
            UpdateLoopType updateLoop,
            InputActionEventType eventType,
            string actionName)
        {
            if (!ReInput.isReady) return;
            _inputEventDelegateActions.Add(callback);
            player0.AddInputEventDelegate(callback, updateLoop, eventType, actionName);
        }

        public void RemoveEvent(Action<InputActionEventData> callback)
        {
            if (!ReInput.isReady) return;
            _inputEventDelegateActions.Remove(callback);
            player0.RemoveInputEventDelegate(callback);
        }
    }
}