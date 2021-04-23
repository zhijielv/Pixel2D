/*
** Created by fengling
** DateTime:    2021-03-21 11:36:57
** Description: 
*/

using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using Framework.Scripts.Singleton;
using Framework.Scripts.UI.Base;
using Framework.Scripts.UI.View;
using Rewired;
using Rewired.Integration.UnityUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private Text leftTopText;
    private Text centerText;
    private UiWidgetBase centerRightText;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        AddManager<UiManager>();
        AddManager<EventManager>();
        AddManager<TimerManager>();
        AddManager<LevelManager>();
        FrameWorkInit();
    }

    private void AddManager<T>() where T : ManagerSingleton<T>
    {
        Constants.AddOrGetComponent(gameObject, typeof(T));
    }

    private void FrameWorkInit()
    {
        DontDestroyOnLoad(transform);
        GameObject mainCanvas = GameObject.FindWithTag("MainCanvas");
        if (mainCanvas == null)
        {
            mainCanvas = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Constants.MainCanvasObj));
            mainCanvas.name = Constants.ReplaceString(mainCanvas.name, "(Clone)", "");
        }

        UiManager.Instance.mainCanvas = mainCanvas.GetComponent<Canvas>();

        GameObject rewiredInputManagerObj = GameObject.Find("Rewired Input Manager");
        if (rewiredInputManagerObj == null)
        {
            rewiredInputManagerObj =
                Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Constants.RewiredInputManagerObj));
            rewiredInputManagerObj.name = Constants.ReplaceString(rewiredInputManagerObj.name, "(Clone)", "");
        }

        GameObject rewiredEventSystemGameObject = GameObject.Find("Rewired Event System");
        if (rewiredEventSystemGameObject == null)
            rewiredEventSystemGameObject = new GameObject {name = "Rewired Event System"};

        RewiredEventSystem rewiredEventSystem =
            (RewiredEventSystem) Constants.AddOrGetComponent(rewiredEventSystemGameObject, typeof(RewiredEventSystem));
        rewiredEventSystem.pixelDragThreshold = 5;

        RewiredStandaloneInputModule rewiredStandaloneInputModule =
            (RewiredStandaloneInputModule) Constants.AddOrGetComponent(rewiredEventSystemGameObject,
                typeof(RewiredStandaloneInputModule));
        rewiredStandaloneInputModule.UseAllRewiredGamePlayers = true;
        rewiredStandaloneInputModule.UseRewiredSystemPlayer = true;
        rewiredStandaloneInputModule.RewiredInputManager =
            rewiredInputManagerObj.GetComponent<InputManager_Base>();
        
        transform.name = "[FrameWork]";
        mainCanvas.transform.SetParent(transform);
        rewiredInputManagerObj.transform.SetParent(transform);
        rewiredEventSystemGameObject.transform.SetParent(transform);
    }

    private void Start()
    {
        // UiManager.Instance.GetWidget<Player_View>();
        // UiManager.Instance.GetWidget<Test_View>();
        // 获取或创建界面
        // UiManager.Instance.GetWidget<Main_View>();
        // 切换Ui
        // Main_View mainView = (Main_View) UiManager.Instance.GetWidget<Main_View>();
        // mainView.ChangePanel<Test_View>();
        // 四种获取ui组件或界面的方法
        // 1
        // leftTopText = (Text) UiManager.Instance.GetWidget<Main_View>(Main_View_Widget.LeftTop_Text);
        // 2
        // centerText = UiManager.Instance.GetWidget(AllViewEnum.Main_View, Main_View_Widget.Center_Text).GetComponent<Text>();
        // centerRightText = UiManager.Instance.GetWidget(AllViewEnum.Main_View, Main_View_Widget.CenterRight_Text);
        // 3
        // UiManager.Instance.GetWidgetComponent<Text>(AllViewEnum.Main_View, Main_View_Widget.CenterTop_Text).text = "";
        // 4
        // Text rightTopText = (Text) UiManager.Instance.GetWidgetObj<Main_View>(Main_View_Widget.RightTop_Text);
        // rightTopText.text = "";
    }

    private void Update()
    {
        // leftTopText.text = System.DateTime.Now.ToString("yyyy-MM-dd dddd");
        // centerText.text = System.DateTime.Now.ToString("HH:mm");
        // centerRightText.TextWrite(System.DateTime.Now.ToString("ss"));
    }
}