using System;
using System.Text;
using UnityEngine;

namespace Framework
{
    public static class Constants
    {
        ////////////////////////////////////////////  Path  ///////////////////////////////////////////////////////
        public static readonly string ScriptableObjectDir = "Assets/Art/UIScriptableObject/";
        public static readonly string ScriptableObjectScriptDir = "Assets/Framework/Scripts/UI/ScriptableObjects/";
        public static readonly string ViewScriptDir = "/Framework/Scripts/UI/View/";
        public static readonly string ViewPrefabDir = "Assets/Art/Prefabs/UI/View/";

        ////////////////////////////////////////////  Name  ///////////////////////////////////////////////////////
        public const string UiScriptableObjectsManager = "Ui Scriptable Objects Manager.asset";
        public const string UiNameSpace = "Framework.Scripts.UI.View.";

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
    }
}