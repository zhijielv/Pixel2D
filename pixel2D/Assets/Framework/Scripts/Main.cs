/*
** Created by fengling
** DateTime:    2021-03-21 11:36:57
** Description: 
*/

using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using Framework.Scripts.Singleton;
using Framework.Scripts.UI.View;
using SRF;
using UnityEngine;

public class Main : ManagerSingleton<Main>
{
    public AllViewEnum firstView;

    // private Text leftTopText;
    // private Text centerText;
    // private UiWidgetBase centerRightText;

    protected new async void Awake()
    {
        base.Awake();
        // 初始化进入的界面
        firstView = Common.IsDebugMode ? AllViewEnum.MaxValue : AllViewEnum.BiaoTi_View;
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad(transform);
        GameObject frameWorkObj = GameObject.Find("[FrameWork]");
        if (frameWorkObj == null)
        {
            transform.name = "[FrameWork]";
            Common.FrameWorkObj = gameObject;
        }
        
        await AddManager<AddressableManager>();
        await AddManager<UiManager>();
        await AddManager<Launcher>();
        await AddManager<RewiredInputEventManager>();
        await AddManager<EventManager>();
        await AddManager<TimerManager>();
        await CreateManager<ObjectManager>();
        await CreateManager<CameraManager>();
        await CreateManager<LevelManager>();
        await CreateManager<Language>(transform);
        await AddManager<FrameWorkDebugMode>();
    }

    // Main上挂管理器
    private async Task AddManager<T>() where T : ManagerSingleton<T>
    {
        gameObject.GetComponentOrAdd<T>();
        await ManagerSingleton<T>.Instance.ManagerInit();
    }

    // 创建空对象挂管理器
    private async Task CreateManager<T>(Transform parent = null) where T : ManagerSingleton<T>
    {
        GameObject managerObj = new GameObject("[" + typeof(T).Name + "]");
        managerObj.GetComponentOrAdd<T>();
        await ManagerSingleton<T>.Instance.ManagerInit();
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