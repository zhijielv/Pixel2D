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
        __UnityEngine_UI,
        TMPCustomScroll,
        CustomPanel,
        __Framework_Scripts_UI_CustomUI,
        TouchController,
        __Rewired_ComponentControls,
        __MaxValue,
    }

    // 不需要自动生成的类型
    public enum IgnoreUI
    {
        View,
    }
}