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
using DG.Tweening;
using Framework.Scripts.Constants;
using HutongGames.PlayMaker;
using Rewired;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Framework.Scripts.UI.View
{
    using Base;
    using System;
    using UnityEngine;
    
    public partial class Player_View : ViewBase
    {
        public string HeroName = "";
        public float speed = 4f;
        public GameObject Player;
        private void OnEnable()
        {
            if (!ReInput.isReady) return;
            Player player = ReInput.players.GetPlayer(0);
            // Subscribe to input events
            // AddInputEventDelegate(TestX, UpdateLoopType.Update, InputActionEventType.AxisActive, "MoveX");
            // AddInputEventDelegate(TestX, UpdateLoopType.Update, InputActionEventType.AxisInactive, "MoveX");
            // AddInputEventDelegate(TestY, UpdateLoopType.Update, InputActionEventType.AxisActive, "MoveY");
            // AddInputEventDelegate(TestY, UpdateLoopType.Update, InputActionEventType.AxisInactive, "MoveY");
            // Rewired按钮，获取Action为Fire，按下和释放的回调；
            AddInputEventDelegate(TestButton, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Fire");
            AddInputEventDelegate(TestButton, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Fire");

            // TestListenerFunc方法监听EventConstants.StartGame事件
            AddEventListener(EventConstants.StartGame, TestListenerFunc);
            
            // 自己拼的ui，监听事件
            AddButtonClickEvent(LoadLevel_Button, LoadLevel);
            AddButtonClickEvent(LoadAvatar_Button, LoadAvatar);
            AddButtonClickEvent(SetSpeed_Button, SetSpeed);
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
            if(Player == null) return;
            Player.transform.GetComponent<Transform>().DOBlendableMoveBy(Vector3.right * data.GetAxis() / 100 * speed, 0.1f);
        }

        public void TestY(InputActionEventData data)
        {
            if(Player == null) return;
            Player.transform.GetComponent<Transform>().DOBlendableMoveBy(Vector3.up * data.GetAxis() / 100 * speed, 0.1f);
        }

        public void TestButton(InputActionEventData data)
        {
            
            // EventManager.Instance.DispatchEvent(EventConstants.StartGame);
        }

        public async void LoadLevel()
        {
            LevelManager.Instance.levelType = LevelType.yanjiang;
            await LevelManager.Instance.LoadLevel();
        }

        public async void LoadAvatar()
        {
            if (Player != null) AddressableManager.Instance.ReleaseInstance(Player);
            Player = await ObjectManager.Instance.LoadPlayerAvatar(HeroName, LevelManager.Instance.transform);
            FsmFloat fsmSpeed = Player.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Speed");
            fsmSpeed = speed;
            Speed_InputField.text = speed.ToString();
        }

        public void SetSpeed()
        {
            if(!Player) return;
            FsmFloat fsmSpeed = Player.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Speed");
            float inputSpeed = Convert.ToSingle(Speed_InputField.text);
            if (inputSpeed > 0)
            {
                fsmSpeed.Value = inputSpeed;
                speed = inputSpeed;
            }
        }
    }
}
