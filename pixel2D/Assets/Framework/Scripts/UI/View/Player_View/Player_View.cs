/*
** Created by fengling
** DateTime:    2021-04-30 11:18:52
** Description: 玩家控制界面
*/

using Framework.Scripts.Manager;
using Cinemachine;
using DG.Tweening;
using HutongGames.PlayMaker;
using Rewired;
using Keyboard = UnityEngine.InputSystem.Keyboard;

namespace Framework.Scripts.UI.View
{
    using Base;
    using System;
    using UnityEngine;

    public sealed partial class Player_View : ViewBase
    {
        public string HeroName = "c01";
        public float speed = 4f;

        private void OnEnable()
        {
            // Rewired
            // 按钮，获取Action为Fire，按下和释放的回调；
            AddInputEventDelegate(TestWheel, UpdateLoopType.Update, InputActionEventType.AxisActive, "Wheel");

            // EventManager 

            // 局部事件 TestListenerFunc方法监听EventConstants.StartGame事件
            AddEventListener(Constants.EventType.StartGame, TestListenerFunc);
            // 全局事件
            // 测试 ObjectManager 的 internalPool
            EventManager.Instance.AddEventListener(Constants.EventType.BagpackChange, TestListenerFunc2);
            EventManager.Instance.RemoveEventListener(Constants.EventType.BagpackChange, TestListenerFunc2);
            EventManager.Instance.AddEventListener(Constants.EventType.BagpackChange, TestListenerFunc2);
            // UIEvent
            // 自己拼的ui，监听事件
            LoadLevel_Button.AddButtonClickEvent(LoadLevel);
            LoadAvatar_Button.AddButtonClickEvent(LoadAvatar);
            SetSpeed_Button.AddButtonClickEvent(SetSpeed);

            LoadLevel();
            LoadAvatar();
        }

        private void Update()
        {
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                ObjectManager.Instance.unitPool.DespawnAll();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                EventManager.Instance.DispatchEvent(this, Constants.EventType.StartGame, new EventData() {Data = 1});
                // test222();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                EventManager.Instance.DispatchEvent(Constants.EventType.BagpackChange, new EventData<int>(1));
                // test222();
            }

            /*if (Input.GetKeyDown(KeyCode.D))
            {
                TimerManager.Instance.Schedule(2.0f, (sender, args) =>
                {
                    Debug.Log(args);
                }, new EventData(){Data = 22});
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                TimerManager.Instance.ScheduleFixed(2.0f, (sender, args) =>
                {
                    EventData<int> eventData = args as EventData<int>;
                    Debug.Log("timer E");
                    if (eventData != null) Debug.Log(eventData.Value);
                }, new EventData<int>(33));
            }*/
        }

        private void TestListenerFunc(object sender, EventArgs eventArgs)
        {
            EventData eventData = eventArgs as EventData;
            Debug.Log($"{sender}    {eventData.Data}");
        }

        private void TestListenerFunc2(object sender, EventArgs eventArgs)
        {
            EventData<int> eventData = eventArgs as EventData<int>;
            Debug.Log($"{sender}    {eventData.Value}");
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveEventListener(Constants.EventType.BagpackChange, TestListenerFunc2);
            Disable();
        }

        public void TestY(InputActionEventData data)
        {
            if (ObjectManager.Instance.mainPlayer == null) return;
            ObjectManager.Instance.mainPlayer.transform.GetComponent<Transform>()
                .DOBlendableMoveBy(Vector3.up * data.GetAxis() / 100 * speed, 0.1f);
        }

        // todo 移动端 双指缩放视野
        // 鼠标滑轮缩放视野
        public void TestWheel(InputActionEventData data)
        {
            if (CameraManager.Instance.playerVCamera == null) return;
            float orthographicSize = CameraManager.Instance.playerVCamera.GetComponent<CinemachineVirtualCamera>()
                .m_Lens.OrthographicSize;
            CameraManager.Instance.playerVCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize =
                Mathf.Clamp(orthographicSize - data.GetAxisDelta() / 2.0f, CameraManager.MINOrthographicSize,
                    CameraManager.MAXOrthographicSize);
            ;
        }

        public void LoadLevel()
        {
            // LevelManager.Instance.levelType = LevelType.yanjiang;
            LevelManager.Instance.InitLevelLoader();
        }

        public async void LoadAvatar()
        {
            if (ObjectManager.Instance.mainPlayer != null)
            {
                CameraManager.Instance.RemoveTarget(ObjectManager.Instance.mainPlayer);
                AddressableManager.Instance.ReleaseInstance(ObjectManager.Instance.mainPlayer);
            }

            await ObjectManager.Instance.LoadPlayerAvatar(HeroName, LevelManager.Instance.transform);
            FsmFloat fsmSpeed = ObjectManager.Instance.mainPlayer.GetComponent<PlayMakerFSM>().FsmVariables
                .FindFsmFloat("Speed");
            fsmSpeed.Value = speed;
            Speed_InputField.text = speed.ToString();

            CameraManager.Instance.CreatePlayerCamera();
            CameraManager.Instance.AddTarget(ObjectManager.Instance.mainPlayer);
        }

        public void SetSpeed()
        {
            if (!ObjectManager.Instance.mainPlayer) return;
            FsmFloat fsmSpeed = ObjectManager.Instance.mainPlayer.GetComponent<PlayMakerFSM>().FsmVariables
                .FindFsmFloat("Speed");
            float inputSpeed = Convert.ToSingle(Speed_InputField.text);
            if (inputSpeed > 0)
            {
                fsmSpeed.Value = inputSpeed;
                speed = inputSpeed;
            }
        }
    }
}