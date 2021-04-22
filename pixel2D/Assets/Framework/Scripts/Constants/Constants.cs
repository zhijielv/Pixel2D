﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.Scripts.Constants
{
    public static class Constants
    {
        ////////////////////////////////////////////  Path  ///////////////////////////////////////////////////////
        public static readonly string ScriptableObjectDir = "Assets/Art/ScriptableObject/UIScriptableObject/";
        public static readonly string ScriptableObjectScriptDir = "Assets/Framework/Scripts/UI/ScriptableObjects/";
        public static readonly string ViewScriptDir = "/Framework/Scripts/UI/View/";
        public static readonly string ViewPrefabDir = "Assets/Art/Prefabs/UI/View/";
        public static readonly string JsonPath = "Assets/Framework/Json/" + "Map.json";

        ////////////////////////////////////////////  Name  ///////////////////////////////////////////////////////
        public const string UiScriptableObjectsManager = "Ui Scriptable Objects Manager.asset";
        public const string UiNameSpace = "Framework.Scripts.UI.View.";
        public const string CustomUiNameSpace = "Framework.Scripts.UI.CustomUI.";

        public const string ArtDirPath = "Assets/Art/";

        ////////////////////////////////////////////  Function  ///////////////////////////////////////////////////////
        public static Component AddOrGetComponent(GameObject go, Type componentType)
        {
            Component c = go.GetComponent(componentType);
            if (c == null)
                c = go.AddComponent(componentType);
            return c;
        }

        public static string ReplaceString(string oldStr, string oldValue, string newValue)
        {
            StringBuilder strBuffer = new StringBuilder();
            int start = 0;
            int tail = 0;

            if (oldStr.IndexOf(oldValue, StringComparison.Ordinal) == -1)
            {
                Debug.Log("没有找到需要替换的关键字符串!");
                return oldStr;
            }

            while (true)
            {
                start = oldStr.IndexOf(oldValue, start, StringComparison.Ordinal);
                if (start == -1)
                {
                    break;
                }

                strBuffer.Append(oldStr.Substring(tail, start - tail));
                strBuffer.Append(newValue);
                start += oldValue.Length;
                tail = start;
            }

            strBuffer.Append(oldStr.Substring(tail));
            return strBuffer.ToString();
        }

#if UNITY_EDITOR
        public static Type GetWidgetTypeByName(string widgetName)
        {
            string[] tmpStrs = widgetName.Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries);
            string typeName = tmpStrs[tmpStrs.Length - 1];

            if (Enum.TryParse(typeName, true, out UIConfig tmpUiType) &&
                !Enum.TryParse(typeName, true, out IgnoreUI ignoreUI))
            {
                if (tmpUiType < UIConfig.___UnityEngine_UI)
                    return AssemblyUtilities.GetTypeByCachedFullName("UnityEngine.UI." + typeName);
                if (tmpUiType < UIConfig.___CustomView)
                    return AssemblyUtilities.GetTypeByCachedFullName(CustomUiNameSpace + widgetName);
                if (tmpUiType < UIConfig.___GameObject)
                    return AssemblyUtilities.GetTypeByCachedFullName("UnityEngine.GameObject");
            }

            return null;
        }
#endif
    }
}