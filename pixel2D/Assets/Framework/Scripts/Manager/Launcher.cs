using System;
using Framework.Scripts.Constants;
using Framework.Scripts.Singleton;
using Framework.Scripts.UI.View;
using Rewired;
using Rewired.Integration.UnityUI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.Scripts.Manager
{
    public class Launcher : ManagerSingleton<Launcher>
    {
        private void Start()
        {
            if (!Common.Initialized)
            {
                StartLauncher();
                Common.Initialized = true;
            }
        }

        private void StartLauncher()
        {
            FrameWorkInit();
            UiManager.Instance.GetWidget<Player_View>();
        }

        private void FrameWorkInit()
        {
            Debug.Log("******************** Framework Init **********************");
            if (Common.MainCanvas == null)
            {
                Addressables.LoadAssetAsync<GameObject>(Constants.Constants.MainCanvasObj).Completed += handle =>
                {
                    Common.MainCanvas = Instantiate(handle.Result, Common.FrameWorkObj.transform, true);
                    Common.MainCanvas.name = Constants.Constants.ReplaceString(Common.MainCanvas.name, "(Clone)", "");
                };
            }

            GameObject rewiredInputManagerObj = GameObject.Find("Rewired Input Manager");
            if (rewiredInputManagerObj == null)
            {
                Addressables.LoadAssetAsync<GameObject>(Constants.Constants.RewiredInputManagerObj).Completed +=
                    handle =>
                    {
                        rewiredInputManagerObj = Instantiate(handle.Result, Common.FrameWorkObj.transform, true);
                        rewiredInputManagerObj.name =
                            Constants.Constants.ReplaceString(rewiredInputManagerObj.name, "(Clone)", "");
                        GameObject rewiredEventSystemGameObject = GameObject.Find("Rewired Event System");
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
                    };
            }
        }
    }
}