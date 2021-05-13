/*
** Created by fengling
** DateTime:    2021-04-30 11:18:52
** Description: 玩家控制界面
*/

using Framework.Scripts.Manager;
using Cinemachine;
using DG.Tweening;
using Framework.Scripts.Constants;
using HutongGames.PlayMaker;
using Rewired;

namespace Framework.Scripts.UI.View
{
    using Base;
    using System;
    using UnityEngine;
    
    public partial class Player_View : ViewBase
    {
        public string HeroName = "c01";
        public float speed = 4f;

        private void OnEnable()
        {
            // Rewired
            // 按钮，获取Action为Fire，按下和释放的回调；
            AddInputEventDelegate(TestWheel, UpdateLoopType.Update, InputActionEventType.AxisActive, "Wheel");

            // EventManager
            // TestListenerFunc方法监听EventConstants.StartGame事件
            AddEventListener(EventConstants.StartGame, TestListenerFunc);
            
            // UIEvent
            // 自己拼的ui，监听事件
            AddButtonClickEvent(LoadLevel_Button, LoadLevel);
            AddButtonClickEvent(LoadAvatar_Button, LoadAvatar);
            AddButtonClickEvent(SetSpeed_Button, SetSpeed);
        }
        
        private void TestListenerFunc(EventData data)
        {
            Debug.Log($"{data.Type}    {data.Data}");
        }

        private void OnDisable()
        {
            Disable();
        }

        public void TestY(InputActionEventData data)
        {
            if(ObjectManager.Instance.mainPlayer == null) return;
            ObjectManager.Instance.mainPlayer.transform.GetComponent<Transform>().DOBlendableMoveBy(Vector3.up * data.GetAxis() / 100 * speed, 0.1f);
        }
        
        // todo 移动端 双指缩放视野
        // 鼠标滑轮缩放视野
        public void TestWheel(InputActionEventData data)
        {
            if(CameraManager.Instance.playerVCamera == null) return;
            float orthographicSize = CameraManager.Instance.playerVCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize;
            CameraManager.Instance.playerVCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 
                Mathf.Clamp(orthographicSize - data.GetAxisDelta() / 2.0f, CameraManager.MINOrthographicSize, CameraManager.MAXOrthographicSize);;
        }

        public async void LoadLevel()
        {
            // LevelManager.Instance.levelType = LevelType.yanjiang;
            await LevelManager.Instance.InitLevelLoader();
        }

        public async void LoadAvatar()
        {
            if (ObjectManager.Instance.mainPlayer != null)
            {
                CameraManager.Instance.RemoveTarget(ObjectManager.Instance.mainPlayer);
                AddressableManager.Instance.ReleaseInstance(ObjectManager.Instance.mainPlayer);
            }
            await ObjectManager.Instance.LoadPlayerAvatar(HeroName, LevelManager.Instance.transform);
            FsmFloat fsmSpeed = ObjectManager.Instance.mainPlayer.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Speed");
            fsmSpeed = speed;
            Speed_InputField.text = speed.ToString();
            
            CameraManager.Instance.CreatePlayerCamera();
            CameraManager.Instance.AddTarget(ObjectManager.Instance.mainPlayer);
        }

        public void SetSpeed()
        {
            if(!ObjectManager.Instance.mainPlayer) return;
            FsmFloat fsmSpeed = ObjectManager.Instance.mainPlayer.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Speed");
            float inputSpeed = Convert.ToSingle(Speed_InputField.text);
            if (inputSpeed > 0)
            {
                fsmSpeed.Value = inputSpeed;
                speed = inputSpeed;
            }
        }
    }
}
