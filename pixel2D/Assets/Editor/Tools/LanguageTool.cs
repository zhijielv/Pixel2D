/*
** Created by fengling
** DateTime:    2021-06-09 13:19:47
** Description: 
*/

using System.Collections.Generic;
using System.Linq;
using DG.DemiEditor;
using Framework.Scripts.Constants;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Editor.Tools
{
    public class LanguageTool
    {
        public class LanguageItem
        {
            [ReadOnly] public string Key;
            [ReadOnly] public int Id;
            public string Description;
            public string Value;

            public LanguageItem(string key, int id, string value)
            {
                Key = key;
                Id = id;
                Value = value;
            }
        }

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

        [ShowInInspector]
        [TableList(ShowPaging = true, ShowIndexLabels = true)]
        [Searchable]
        [LabelText("语言列表(右侧‘≡’展开查找)")]
        public List<LanguageItem> LanguageItems = new List<LanguageItem>();

        public LanguageTool()
        {
            ReadJson();
        }

        [TitleGroup("Add Language")]
        [Button(ButtonSizes.Large, ButtonStyle.Box)]
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

        [TitleGroup("Export Json")] public LanguageEnum LanguageEnum = LanguageEnum.Chinese;

        [InlineButton("ExportJson")] [TitleGroup("Export Json")]
        public string LanguageKey = "zh-CN";

        public void ExportJson(string languageKey)
        {
            if (languageKey.IsNullOrEmpty() || null == languageKey)
            {
                EditorUtility.DisplayDialog("添加错误", "请输入key", "确定");
                return;
            }

            string tmpDirectory = Constants.JsonFoldreDir + "Language/" + LanguageEnum;
            if (!System.IO.Directory.Exists(tmpDirectory)) System.IO.Directory.CreateDirectory(tmpDirectory);

            // 导出json
            JsonHelper.JsonWriter(LanguageItems, "Language/" + LanguageEnum + "/" + languageKey);
            AddressableAssetsTool.AddAssets2AddressGroup("t:TextAsset", tmpDirectory, LanguageEnum.ToString());
        }

        [TitleGroup("Export Json")]
        [Button(ButtonSizes.Large)]
        public void ReadJson()
        {
            LanguageItems = JsonHelper.JsonReader<LanguageItem>("Language/" + LanguageEnum + "/" + LanguageKey);
        }

        [ShowInInspector]
        public ErrorCodeLocalization jsonClass;
        [Button]
        public void GenerateErrorCode()
        {
            jsonClass = JsonHelper.GetJsonClass<ErrorCodeLocalization>("ErrorCodeLocalization");
        }
    }

    public class ErrorCodeLocalization
    {
        public string Tool;
        public string Version;
        [ShowInInspector]
        public List<ErrorCodeLocalizationItem> Sheet1;
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
}