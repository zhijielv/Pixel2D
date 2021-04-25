//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Data;
using Framework.Scripts.Manager;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;
using Framework.Scripts.Constants;
using Rewired;
using Sirenix.Utilities;
using UnityEngine;
namespace Framework.Scripts.UI.View
{
    using Base;
    using System;
    using UnityEngine;
    
    public class Player_View : ViewBase
    {
        // member
		public UnityEngine.UI.Button LoadLevel_Button;
        // member end
        internal override object GetWidget(string widgetName)
        {
            if (!Enum.TryParse(widgetName, true, out Player_View_Widget _))
            {
                // Debug.LogError(gameObject.name + " has not widget : " + widgetName);
                return null;
            }
            else
            {
                return base.GetWidget(widgetName);
            }
        }
        
        private void OnEnable()
        {
            if (!ReInput.isReady) return;
            Player player = ReInput.players.GetPlayer(0);
            // Subscribe to input events
            AddInputEventDelegate(TestX, UpdateLoopType.Update, InputActionEventType.AxisActive, "MoveX");
            AddInputEventDelegate(TestX, UpdateLoopType.Update, InputActionEventType.AxisInactive, "MoveX");
            AddInputEventDelegate(TestY, UpdateLoopType.Update, InputActionEventType.AxisActive, "MoveY");
            AddInputEventDelegate(TestY, UpdateLoopType.Update, InputActionEventType.AxisInactive, "MoveY");
            AddInputEventDelegate(TestButton, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Fire");
            AddInputEventDelegate(TestButton, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Fire");

            AddEventListener(EventConstants.StartGame, TestListenerFunc);
            
            AddButtonClickEvent(LoadLevel_Button, LoadLevel);
        }
        
        private void TestListenerFunc(EventData data)
        {
            Debug.Log(GlobalConfig<UiScriptableObjectsManager>.Instance.UiScriptableObjectsList.Length);
            Debug.Log($"{data.Type}    {data.Data}");
        }

        private void OnDisable()
        {
            Disable();
        }

        public void TestX(InputActionEventData data)
        {
        }

        public void TestY(InputActionEventData data)
        {
        }

        public void TestButton(InputActionEventData data)
        {
            EventManager.Instance.DispatchEvent(EventConstants.StartGame);
        }

        public async void LoadLevel()
        {
            LevelManager.Instance.levelType = LevelType.yanjiang;
            await LevelManager.Instance.LoadLevel(Common.MainCanvas.transform);
        }
    }
}
