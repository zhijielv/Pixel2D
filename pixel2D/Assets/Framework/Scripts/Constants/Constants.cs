using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.Utilities;
using UnityEngine;

namespace Framework.Scripts.Constants
{
    public static class Constants
    {
        ////////////////////////////////////////////  Path  ///////////////////////////////////////////////////////
        public static readonly string ScriptableObjectDir = "Assets/Art/ScriptableObject/UIScriptableObject/";
        public static readonly string ScriptableObjectScriptDir = "Assets/Framework/Scripts/UI/ScriptableObjects/";
        public static readonly string ViewScriptDir = "/Framework/Scripts/UI/View/";
        public static readonly string ViewPrefabDir = "Assets/Art/Prefabs/UI/View/";
        public static readonly string LevelPrefabDir = "Assets/Art/Prefabs/Level/";

        ////////////////////////////////////////////  GameObject  ///////////////////////////////////////////////////////
        public const string MainCanvasObj = "_MainCanvas";
        public const string RewiredInputManagerObj = "Rewired Input Manager";
        
        ////////////////////////////////////////////  Name  ///////////////////////////////////////////////////////
        public const string UiScriptableObjectsManager = "Ui Scriptable Objects Manager.asset";
        public const string UiNameSpace = "Framework.Scripts.UI.View.";
        public const string CustomUiNameSpace = "Framework.Scripts.UI.CustomUI.";
        public const string LevelJson = "Level";
        public const string MapJson = "Map";

        public const string ArtDirPath = "Assets/Art/";
        public const string UIAssets = "UIScriptableObject/";
        public const string UIView = "UIView/";
        public const string ObjectUnit = "ObjectUnit";

        ////////////////////////////////////////////  Function  ///////////////////////////////////////////////////////
        // 添加或获取组件
        public static Component AddOrGetComponent(GameObject go, Type componentType)
        {
            Component c = go.GetComponent(componentType);
            if (c == null)
                c = go.AddComponent(componentType);
            return c;
        }

        // 字符串替换
        public static string ReplaceString(string oldStr, string oldValue, string newValue)
        {
            StringBuilder strBuffer = new StringBuilder();
            int start = 0;
            int tail = 0;

            if (oldStr.IndexOf(oldValue, StringComparison.Ordinal) == -1)
            {
                Debug.Log("没有找到需要替换的字符串!");
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

        // 单帧处理协程
        public static IEnumerator ToFixedCoroutine(IEnumerator enumerator)
        {
            var parentsStack = new Stack<IEnumerator>();
            var currentEnumerator = enumerator;

            parentsStack.Push(currentEnumerator);

            while (parentsStack.Count > 0)
            {
                currentEnumerator = parentsStack.Pop();

                while (currentEnumerator.MoveNext())
                {
                    var subEnumerator = currentEnumerator.Current as IEnumerator;
                    if (subEnumerator != null)
                    {
                        parentsStack.Push(currentEnumerator);
                        currentEnumerator = subEnumerator;
                    }
                    else
                    {
                        if (currentEnumerator.Current is bool && (bool)currentEnumerator.Current) continue;
                        yield return currentEnumerator.Current;
                    }
                }
            }
        }

#if UNITY_EDITOR
        public static Type GetWidgetTypeByName(string widgetName)
        {
            string[] tmpStrs = widgetName.Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries);
            string typeName = tmpStrs[tmpStrs.Length - 1];
            Type byName = null;
            if (!Enum.TryParse(typeName, true, out UIConfig tmpUiType) ||
                Enum.TryParse(typeName, true, out IgnoreUI ignoreUI)) return null;
            for (UIConfig i = tmpUiType; i < UIConfig.__MaxValue; i++)
            {
                if (!i.ToString().StartsWith("__")) continue;
                string[] tmpNameSpace = i.ToString().Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder nameSpace = new StringBuilder();
                foreach (var t in tmpNameSpace)
                {
                    nameSpace.Append(t);
                    nameSpace.Append(".");
                }

                byName = AssemblyUtilities.GetTypeByCachedFullName(nameSpace + typeName);
                break;
            }

            return byName;
        }
#endif
    }
}