/*
** Created by fengling
** DateTime:    2021-06-09 13:19:47
** Description: 
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.DemiEditor;
using Framework.Scripts.Constants;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools
{
    public class LanguageTool
    {
        /*
        public class LanguageToggle
        {
            public LanguageEnum LanguageEnum;
            public bool isShow;
            public LanguageToggle(LanguageEnum languageEnum, bool isShow)
            {
                LanguageEnum = languageEnum;
                this.isShow = isShow;
            }
        }

        [ShowInInspector]
        [TableMatrix(HorizontalTitle = "语言", DrawElementMethod = "ShowLanguage", HideColumnIndices = true,
            HideRowIndices = true)]
        public LanguageToggle[,] LanguageToggles = new LanguageToggle[(int) LanguageEnum.MaxValue, 1];
        
        public static LanguageToggle ShowLanguage(Rect rect, LanguageToggle[,] languageToggles, int x, int y)
        {
            EditorGUI.LabelField(rect.Padding(15,0), languageToggles[x, y].LanguageEnum.ToString());
            EditorGUI.Toggle(rect, languageToggles[x, y].isShow);
            return languageToggles[x, y];
            
            // 赋值
            int tmpIndex = 0;
            foreach (LanguageEnum language in Enum.GetValues(typeof(LanguageEnum)))
            {
                if (language == LanguageEnum.MaxValue) return;
                LanguageToggles[tmpIndex, 0] = new LanguageToggle(language, true);
                tmpIndex++;
            }
        }*/

        #region Export Json

        [FoldoutGroup("Export Json", true)]
        [ShowInInspector]
        [TableList(ShowPaging = true, ShowIndexLabels = true)]
        [Searchable]
        [LabelText("语言列表(右侧‘≡’展开查找)")]
        public List<LanguageItem> LanguageItems = new List<LanguageItem>();

        [ShowInInspector, ReadOnly]
        public const string LanguageDirectory = "Assets/Framework/Language/";
        public LanguageTool()
        {
            ReadJson();
        }

        [FoldoutGroup("Export Json")]
        [Button("添加语言", ButtonSizes.Large, ButtonStyle.Box)]
        public void AddLanguage(string key, string description, string value)
        {
            if (null == key || key.IsNullOrEmpty())
            {
                EditorUtility.DisplayDialog("添加错误", "请输入key", "确定");
                return;
            }

            if (LanguageItems.Any(languageItem => languageItem.Key == key))
            {
                EditorUtility.DisplayDialog("添加错误", "key同名", "确定");
                return;
            }

            LanguageItem item = new LanguageItem(key, LanguageItems.Count, value) {Description = description};
            LanguageItems.Add(item);
        }

        // todo 切换时改变LanguageKey
        [FoldoutGroup("Export Json")] public LanguageEnum LanguageEnum = Common.Language;

        [FoldoutGroup("Export Json")] [InlineButton("ExportJson", "导出语言表")]
        public string LanguageKey = "zh-CN";

        public void ExportJson(string languageKey)
        {
            if (languageKey.IsNullOrEmpty() || null == languageKey)
            {
                EditorUtility.DisplayDialog("添加错误", "请输入key", "确定");
                return;
            }

            string tmpDirectory = LanguageDirectory + LanguageEnum;
            if (!System.IO.Directory.Exists(tmpDirectory)) Directory.CreateDirectory(tmpDirectory);

            // 导出json
            JsonHelper.JsonWriter(LanguageItems, tmpDirectory + "/" + languageKey, false);
            AddressableAssetsTool.AddAssets2AddressGroup("t:TextAsset", tmpDirectory, LanguageEnum.ToString());
        }

        [FoldoutGroup("Export Json")]
        [HorizontalGroup("Export Json/ExportButton")]
        [Button("读取json表", ButtonSizes.Large)]
        public void ReadJson()
        {
            JsonHelper.JsonReader(out LanguageItems, "Assets/Framework/Language/" + LanguageEnum + "/" + LanguageKey, false);
        }

        [FoldoutGroup("Export Json")]
        [HorizontalGroup("Export Json/ExportButton")]
        [Button("导出所有语言", ButtonSizes.Large)]
        public void ExportAllLanguage()
        {
            string languageKey = "";
            foreach (LanguageEnum language in Enum.GetValues(typeof(LanguageEnum)))
            {
                switch (language)
                {
                    case LanguageEnum.Chinese :
                        languageKey = "zh-CN";
                        break;
                    case LanguageEnum.English:
                        languageKey = "en-US";
                        break;
                    case LanguageEnum.ChineseTraditional:
                        languageKey = "zh-TW";
                        break;
                    case LanguageEnum.Japanese:
                        languageKey = "ja-JP";
                        break;
                    case LanguageEnum.Russian:
                        languageKey = "ru-RU";
                        break;
                    case LanguageEnum.German:
                        languageKey = "de-DE";
                        break;
                    case LanguageEnum.Spanish:
                        languageKey = "es-ES";
                        break;
                    case LanguageEnum.Korean:
                        languageKey = "ko-KR";
                        break;
                    case LanguageEnum.Portuguese:
                        languageKey = "pt-PT";
                        break;
                    case LanguageEnum.French:
                        languageKey = "fr-FR";
                        break;
                    case LanguageEnum.Indonesian:
                        languageKey = "en-ID";
                        break;
                    case LanguageEnum.Polish:
                        languageKey = "pl-PL";
                        break;
                    // todo 不知道是哪国
                    case LanguageEnum.FarsiIran:
                        languageKey = "FarsiIran";
                        break;
                    case LanguageEnum.ArabicEgypt:
                        languageKey = "en-XA";
                        break;
                    case LanguageEnum.MaxValue:
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                string tmpDirectory = LanguageDirectory + language;
                // 导出json
                JsonHelper.JsonWriter(LanguageItems, LanguageDirectory + language + "/" + languageKey, false);
                AddressableAssetsTool.AddAssets2AddressGroup("t:TextAsset", tmpDirectory, language.ToString());
            }
        }

        #endregion

        /*#region ExportErrorCode

        [FoldoutGroup("ExportErrorCode")] [ShowInInspector]
        public ErrorCodeLocalization jsonClass;

        /// <summary>
        /// 读取ErrorCodeLocalization.json文件并导出相关语言
        /// </summary>
        [FoldoutGroup("ExportErrorCode")]
        [Button("格式化错误码", ButtonSizes.Large)]
        public void GenerateErrorCode()
        {
            jsonClass = JsonHelper.GetJsonClass<ErrorCodeLocalization>("ErrorCodeLocalization");
        }

        [FoldoutGroup("ExportErrorCode")]
        [Button("导出错误码", ButtonSizes.Large)]
        // [ButtonGroup("Generate")]
        public void ExportErrorCode()
        {
            foreach (LanguageEnum language in Enum.GetValues(typeof(LanguageEnum)))
            {
                if (language == LanguageEnum.MaxValue) return;
                List<ErrorCodeItem> list = new List<ErrorCodeItem>();
                foreach (ErrorCodeLocalizationItem localizationItem in jsonClass.Sheet1)
                {
                    Type viewType = localizationItem.GetType();
                    FieldInfo[] fieldInfos = viewType
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                    string value = "";
                    // 赋值 view 组件
                    foreach (var fieldInfo in fieldInfos)
                    {
                        if (!language.ToString().Equals(fieldInfo.Name)) continue;
                        value = (string) fieldInfo.GetValue(localizationItem);
                        break;
                    }

                    ErrorCodeItem codeItem = new ErrorCodeItem(localizationItem.key,
                        Convert.ToInt32(localizationItem.value), localizationItem.Description,
                        value);

                    list.Add(codeItem);
                    JsonHelper.JsonWriter(list, LanguageDirectory + language + "/" + "ErrorCodeLocalization", false);
                }
            }
        }

        #endregion*/
    }

    public class ErrorCodeLocalization
    {
        public string Tool;
        public string Version;
        [ShowInInspector] public List<ErrorCodeLocalizationItem> Sheet1;
    }

    public class ErrorCodeLocalizationItem
    {
        public string key;
        public string value;
        public string Description;
        public string English;
        public string Chinese;
        public string ChineseTraditional;
        public string Japanese;
        public string Russian;
        public string German;
        public string Spanish;
        public string Korean;
        public string Portuguese;
        public string French;
        public string Indonesian;
        public string Polish;
        public string FarsiIran;
        public string ArabicEgypt;
    }

    public class ErrorCodeItem
    {
        public string key;
        public int id;
        public string description;
        public string value;

        public ErrorCodeItem(string key, int id, string description, string value)
        {
            this.key = key;
            this.id = id;
            this.description = description;
            this.value = value;
        }
    }
}