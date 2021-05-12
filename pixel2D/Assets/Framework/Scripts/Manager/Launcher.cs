using System;
using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Singleton;
using Framework.Scripts.UI.View;
using Rewired;
using Rewired.Integration.UnityUI;
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
            UiManager.Instance.GetWidget<Player_View>();
        }

        private async Task FrameWorkInit()
        {
            Debug.Log("******************** Framework Init **********************");
            if (Common.MainCanvas == null)
            {
                Common.MainCanvas = await AddressableManager.Instance.Instantiate(Constants.Constants.MainCanvasObj,
                    Common.FrameWorkObj.transform);
                Common.MainCanvas.name =
                    Constants.Constants.ReplaceString(Common.MainCanvas.name, "(Clone)", "");
            }

            // 实例化Rewired Input Manager
            GameObject rewiredInputManagerObj = GameObject.Find("Rewired Input Manager");
            if (rewiredInputManagerObj == null)
            {
                rewiredInputManagerObj =
                    await AddressableManager.Instance.Instantiate(Constants.Constants.RewiredInputManagerObj,
                        Common.FrameWorkObj.transform);
                rewiredInputManagerObj.name =
                    Constants.Constants.ReplaceString(rewiredInputManagerObj.name, "(Clone)", "");
                GameObject rewiredEventSystemGameObject = GameObject.Find("Rewired Event System");
                // 创建Rewired Event System
                if (rewiredEventSystemGameObject == null)
                    rewiredEventSystemGameObject = new GameObject {name = "Rewired Event System"};
                rewiredEventSystemGameObject.transform.SetParent(Common.FrameWorkObj.transform);

                RewiredEventSystem rewiredEventSystem =
                    (RewiredEventSystem) Constants.Constants.AddOrGetComponent(rewiredEventSystemGameObject,
                        typeof(RewiredEventSystem));
                rewiredEventSystem.pixelDragThreshold = 5;

                RewiredStandaloneInputModule rewiredStandaloneInputModule =
                    (RewiredStandaloneInputModule) Constants.Constants.AddOrGetComponent(
                        rewiredEventSystemGameObject,
                        typeof(RewiredStandaloneInputModule));
                rewiredStandaloneInputModule.UseAllRewiredGamePlayers = true;
                rewiredStandaloneInputModule.UseRewiredSystemPlayer = true;
                rewiredStandaloneInputModule.RewiredInputManager =
                    rewiredInputManagerObj.GetComponent<InputManager_Base>();
            }
        }
    }
}