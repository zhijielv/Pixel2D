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
    
    public sealed partial class TestView : ViewBase
    {
        // member
        [FoldoutGroup("Member", true)]
        public UnityEngine.UI.Button Test_Button;
        [FoldoutGroup("Member", true)]
        public UnityEngine.UI.Button Test3_Button;
        [FoldoutGroup("Member", true)]
        public Framework.Scripts.UI.ScriptableObjects.PanelScriptableObjectBase Test_View_ScriptableObject;
        // member end
        internal override object GetWidget(string widgetName)
        {
            if (!Enum.TryParse(widgetName, true, out Test_View_Widget _))
            {
                // Debug.LogError(gameObject.name + " has not widget : " + widgetName);
                return gameObject;
            }
            else
            {
                return base.GetWidget(widgetName);
            }
        }
    }
}
