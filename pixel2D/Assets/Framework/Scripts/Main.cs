/*
** Created by fengling
** DateTime:    2021-03-21 11:36:57
** Description: 
*/

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using Framework.Scripts.Singleton;
using Framework.Scripts.UI.Base;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private Text leftTopText;
    private Text centerText;
    private UiWidgetBase centerRightText;

    private async void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad(transform);
        GameObject frameWorkObj = GameObject.Find("[FrameWork]");
        if (frameWorkObj == null)
        {
            transform.name = "[FrameWork]";
            Common.FrameWorkObj = gameObject;
        }

        await AddManager<AddressableManager>();
        await AddManager<ObjectManager>();
        await AddManager<EventManager>();
        await AddManager<TimerManager>();
        await AddManager<UiManager>();
        await AddManager<Launcher>();
        await AddManager<CameraManager>();
        await CreateManager<LevelManager>();
    }

    private async Task AddManager<T>() where T : ManagerSingleton<T>
    {
        Constants.AddOrGetComponent(gameObject, typeof(T));
        await ManagerSingleton<T>.Instance.Init();
    }

    private async Task CreateManager<T>(Transform parent = null) where T : ManagerSingleton<T>
    {
        GameObject managerObj = new GameObject(typeof(T).Name);
        Constants.AddOrGetComponent(managerObj, typeof(T));
        await ManagerSingleton<T>.Instance.Init();
        managerObj.transform.SetParent(parent);
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