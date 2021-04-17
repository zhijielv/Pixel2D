namespace Framework.Scripts.Constants
{
    // 保存需要Generate的UI类型
    public enum UIConfig
    {
        Text,
        Button,
        Image,
        ___UnityEngine_UI,
        TMPCustomScroll,
        ___CustomView,
        Panel,
        ___GameObject,
    }

    // 不需要自动生成的类型
    public enum IgnoreUI
    {
        View,
    }
}