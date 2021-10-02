namespace Framework.Scripts.Constants
{
    // 保存需要Generate的UI类型
    // 向下查找第一个__开头的是namespace
    public enum UIConfig
    {
        Text,
        Button,
        Image,
        InputField,
        Dropdown,
        UnityEngineUI,
        TMPCustomScroll,
        CustomPanel,
        FrameworkScriptsUICustomUI,
        TouchController,
        RewiredComponentControls,
        MaxValue,
    }

    // 不需要自动生成的类型
    public enum IgnoreUI
    {
        View,
    }
}