using Framework.Scripts.Constants;
using Rewired;
using Rewired.Integration.UnityUI;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools
{
    public class HierarchyTool
    {
        [MenuItem("GameObject/Scene Init", false, -100)]
        public static void LoadMainCanvas()
        {
            GameObject mainCanvas = GameObject.FindWithTag("MainCanvas");
            if (mainCanvas == null)
            {
                mainCanvas = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Constants.MainCanvasObj));
                mainCanvas.name = Constants.ReplaceString(mainCanvas.name, "(Clone)", "");
            }

            GameObject rewiredInputManagerObj = GameObject.Find("Rewired Input Manager");
            if (rewiredInputManagerObj == null)
            {
                rewiredInputManagerObj =
                    Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Constants.RewiredInputManagerObj));
                rewiredInputManagerObj.name = Constants.ReplaceString(rewiredInputManagerObj.name, "(Clone)", "");
            }

            GameObject rewiredEventSystemGameObject = GameObject.Find("Rewired Event System");
            if (rewiredEventSystemGameObject == null)
                rewiredEventSystemGameObject = new GameObject {name = "Rewired Event System"};

            RewiredEventSystem rewiredEventSystem = (RewiredEventSystem) Constants.AddOrGetComponent(rewiredEventSystemGameObject, typeof(RewiredEventSystem));
            rewiredEventSystem.pixelDragThreshold = 5;

            RewiredStandaloneInputModule rewiredStandaloneInputModule =
                (RewiredStandaloneInputModule) Constants.AddOrGetComponent(rewiredEventSystemGameObject,
                    typeof(RewiredStandaloneInputModule));
            rewiredStandaloneInputModule.UseAllRewiredGamePlayers = true;
            rewiredStandaloneInputModule.UseRewiredSystemPlayer = true;
            rewiredStandaloneInputModule.RewiredInputManager =
                rewiredInputManagerObj.GetComponent<InputManager_Base>();
            
            GameObject frameWorkObj = GameObject.Find("[FrameWork]");
            if(frameWorkObj == null) frameWorkObj = new GameObject("[FrameWork]");
            Constants.AddOrGetComponent(frameWorkObj, typeof(Main));

            mainCanvas.transform.SetParent(frameWorkObj.transform);
            rewiredInputManagerObj.transform.SetParent(frameWorkObj.transform);
            rewiredEventSystemGameObject.transform.SetParent(frameWorkObj.transform);
        }
    }
}