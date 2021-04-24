using Framework.Scripts.Constants;
using Rewired;
using Rewired.Integration.UnityUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Editor.Tools
{
#if UNITY_EDITOR
    public class HierarchyTool
    {
        [MenuItem("GameObject/Scene Init", false, -100)]
        public static void LoadMainCanvas()
        {
            GameObject frameWorkObj = GameObject.Find("[FrameWork]");
            if (frameWorkObj == null) frameWorkObj = new GameObject("[FrameWork]");
            Constants.AddOrGetComponent(frameWorkObj, typeof(Main));

            GameObject rewiredEventSystemGameObject = GameObject.Find("Rewired Event System");
            if (rewiredEventSystemGameObject == null)
                rewiredEventSystemGameObject = new GameObject {name = "Rewired Event System"};
            rewiredEventSystemGameObject.transform.SetParent(frameWorkObj.transform);

            RewiredEventSystem rewiredEventSystem =
                (RewiredEventSystem) Constants.AddOrGetComponent(rewiredEventSystemGameObject,
                    typeof(RewiredEventSystem));
            rewiredEventSystem.pixelDragThreshold = 5;

            RewiredStandaloneInputModule rewiredStandaloneInputModule =
                (RewiredStandaloneInputModule) Constants.AddOrGetComponent(rewiredEventSystemGameObject,
                    typeof(RewiredStandaloneInputModule));
            rewiredStandaloneInputModule.UseAllRewiredGamePlayers = true;
            rewiredStandaloneInputModule.UseRewiredSystemPlayer = true;


            GameObject mainCanvas = GameObject.FindWithTag("MainCanvas");
            if (mainCanvas == null)
            {
                Addressables.LoadAssetAsync<GameObject>(Constants.MainCanvasObj).Completed += handle =>
                {
                    mainCanvas = Object.Instantiate(handle.Result, frameWorkObj.transform, true);
                    mainCanvas.name = Constants.ReplaceString(mainCanvas.name, "(Clone)", "");
                };
            }

            GameObject rewiredInputManagerObj = GameObject.Find("Rewired Input Manager");
            if (rewiredInputManagerObj == null)
            {
                Addressables.LoadAssetAsync<GameObject>(Constants.RewiredInputManagerObj).Completed +=
                    handle =>
                    {
                        rewiredInputManagerObj = Object.Instantiate(handle.Result, frameWorkObj.transform, true);
                        rewiredInputManagerObj.name =
                            Constants.ReplaceString(rewiredInputManagerObj.name, "(Clone)", "");
                        rewiredStandaloneInputModule.RewiredInputManager =
                            rewiredInputManagerObj.GetComponent<InputManager_Base>();
                    };
            }
        }
    }
#endif
}