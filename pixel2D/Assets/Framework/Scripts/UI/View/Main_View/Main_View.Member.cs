//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Framework.Scripts.UI.View
{
    using Base;
    using System;
    using UnityEngine;
    using Sirenix.OdinInspector;
    
    public sealed partial class Main_View : ViewBase
    {
        // member
        [FoldoutGroup("Member")]
        public UnityEngine.UI.Image BG_Image;
        [FoldoutGroup("Member")]
        public Framework.Scripts.UI.CustomUI.CustomPanel Top_CustomPanel;
        [FoldoutGroup("Member")]
        public UnityEngine.UI.Text LeftTop_Text;
        [FoldoutGroup("Member")]
        public UnityEngine.UI.Text CenterTop_Text;
        [FoldoutGroup("Member")]
        public UnityEngine.UI.Text RightTop_Text;
        [FoldoutGroup("Member")]
        public UnityEngine.UI.Button Setting_Button;
        [FoldoutGroup("Member")]
        public Framework.Scripts.UI.CustomUI.CustomPanel Center_CustomPanel;
        [FoldoutGroup("Member")]
        public UnityEngine.UI.Text Center_Text;
        [FoldoutGroup("Member")]
        public UnityEngine.UI.Text CenterRight_Text;
        [FoldoutGroup("Member")]
        public Framework.Scripts.UI.CustomUI.CustomPanel Bottom_CustomPanel;
        [FoldoutGroup("Member")]
        public Framework.Scripts.UI.ScriptableObjects.PanelScriptableObjectBase Main_View_ScriptableObject;
        // member end
        internal override object GetWidget(string widgetName)
        {
            if (!Enum.TryParse(widgetName, true, out Main_View_Widget _))
            {
                // Debug.LogError(gameObject.name + " has not widget : " + widgetName);
                return null;
            }
            else
            {
                return base.GetWidget(widgetName);
            }
        }
    }
}
