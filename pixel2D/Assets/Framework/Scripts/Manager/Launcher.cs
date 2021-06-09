﻿using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Singleton;
using Framework.Scripts.UI.View;
using Rewired;
using Rewired.Integration.UnityUI;
using SRF;
using UnityEngine;

namespace Framework.Scripts.Manager
{
    public class Launcher : ManagerSingleton<Launcher>
    {
        private async void Start()
        {
            if (Common.Initialized) return;
            await FrameWorkInit();
            StartLauncher();
            Common.Initialized = true;
        }

        // 项目初始化进入场景
        private void StartLauncher()
        {
            Debug.Log("******************** start launcher ********************");
            if (Common.IsDebugMode)
            {
                if (Main.Instance.firstView != AllViewEnum.MaxValue)
                {
                    UiManager.Instance.GetWidget(Main.Instance.firstView);
                }
            }
            else
            {
                UiManager.Instance.GetWidget<Player_View>();
            }
        }

        private async Task FrameWorkInit()
        {
            Debug.Log("******************** Framework Init **********************");
            if (Common.MainCanvas == null)
            {
                Common.MainCanvas = AddressableManager.Instance.Instantiate(Constants.Constants.MainCanvasObj,
                    Common.FrameWorkObj.transform);
                Common.MainCanvas.name =
                    Constants.Constants.ReplaceString(Common.MainCanvas.name, "(Clone)", "");
            }

            // 实例化Rewired Input Manager
            // GameObject rewiredInputManagerObj = GameObject.Find("Rewired Input Manager");
            if (Common.RewiredInputManager == null)
            {
                Common.RewiredInputManager = AddressableManager.Instance.Instantiate(
                    Constants.Constants.RewiredInputManagerObj,
                    Common.FrameWorkObj.transform);
                Common.RewiredInputManager.name =
                    Constants.Constants.ReplaceString(Common.RewiredInputManager.name, "(Clone)", "");
                GameObject rewiredEventSystemGameObject = GameObject.Find("Rewired Event System");
                // 创建Rewired Event System
                if (rewiredEventSystemGameObject == null)
                    rewiredEventSystemGameObject = new GameObject {name = "Rewired Event System"};
                rewiredEventSystemGameObject.transform.SetParent(Common.FrameWorkObj.transform);

                RewiredEventSystem rewiredEventSystem =
                    rewiredEventSystemGameObject.GetComponentOrAdd<RewiredEventSystem>();
                rewiredEventSystem.pixelDragThreshold = 5;

                RewiredStandaloneInputModule rewiredStandaloneInputModule =
                    rewiredEventSystemGameObject.GetComponentOrAdd<RewiredStandaloneInputModule>();
                rewiredStandaloneInputModule.UseAllRewiredGamePlayers = true;
                rewiredStandaloneInputModule.UseRewiredSystemPlayer = true;
                rewiredStandaloneInputModule.RewiredInputManager =
                    Common.RewiredInputManager.GetComponent<InputManager_Base>();
            }
        }
    }
}