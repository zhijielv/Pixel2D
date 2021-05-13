using System;
using System.Collections.Generic;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using Framework.Scripts.UI.Base;
using Framework.Scripts.UI.CustomUI;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;    
#endif
using UnityEngine;
// 用来保存数据
namespace Framework.Scripts.UI.ScriptableObjects
{
    public class PanelScriptableObjectBase : SerializedScriptableObject
    {
        [ReadOnly] public List<string> widgetList = new List<string>();
    }
}