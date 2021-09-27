/*
** Created by fengling
** DateTime:    2021-06-09 17:57:26
** Description: TODO 读取表
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.Singleton;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;

namespace Framework.Scripts.Constants
{
    public enum LanguageEnum
    {
        English,
        Chinese,
        ChineseTraditional,
        Japanese,
        Russian,
        German,
        Spanish,
        Korean,
        Portuguese,
        French,
        Indonesian,
        Polish,
        FarsiIran,
        ArabicEgypt,
        MaxValue,
    }

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

    public class Language : ManagerSingleton<Language>
    {
        [ShowInInspector] private static List<LanguageItem> _list;
        [ShowInInspector] private string _languageKey;

        public override Task ManagerInit()
        {
            switch (Common.Language)
            {
                case LanguageEnum.Chinese:
                    _languageKey = "zh-CN";
                    break;
                case LanguageEnum.English:
                    _languageKey = "en-US";
                    break;
                case LanguageEnum.ChineseTraditional:
                    _languageKey = "zh-TW";
                    break;
                case LanguageEnum.Japanese:
                    _languageKey = "ja-JP";
                    break;
                case LanguageEnum.Russian:
                    _languageKey = "ru-RU";
                    break;
                case LanguageEnum.German:
                    _languageKey = "de-DE";
                    break;
                case LanguageEnum.Spanish:
                    _languageKey = "es-ES";
                    break;
                case LanguageEnum.Korean:
                    _languageKey = "ko-KR";
                    break;
                case LanguageEnum.Portuguese:
                    _languageKey = "pt-PT";
                    break;
                case LanguageEnum.French:
                    _languageKey = "fr-FR";
                    break;
                case LanguageEnum.Indonesian:
                    _languageKey = "en-ID";
                    break;
                case LanguageEnum.Polish:
                    _languageKey = "pl-PL";
                    break;
                // todo 不知道是哪国
                case LanguageEnum.FarsiIran:
                    _languageKey = "FarsiIran";
                    break;
                case LanguageEnum.ArabicEgypt:
                    _languageKey = "en-XA";
                    break;
                case LanguageEnum.MaxValue:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            JsonHelper.JsonReader(out _list, _languageKey);
            return base.ManagerInit();
        }

        public static string Get(string key)
        {
            if (_list == null || _list.Count == 0) return null;
            foreach (LanguageItem item in _list)
            {
                if(!item.Key.Equals(key)) continue;
                return item.Value;
            }

            return null;
        }
        
        public static string Get(int id)
        {
            if (_list == null || _list.Count == 0) return null;
            foreach (LanguageItem item in _list)
            {
                if(!item.Id.Equals(id)) continue;
                return item.Value;
            }

            return null;
        }
    }
}