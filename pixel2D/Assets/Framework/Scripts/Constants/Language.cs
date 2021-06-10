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
using UnityEngine.AddressableAssets;

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
    
    public class Language<T> where T : class, new()
    {
        public string JsonName;
        private List<T> _list;
        private string _languageKey;

        public Language(string jsonName)
        {
            JsonName = jsonName;
            Init(jsonName);
        }

        private void Init(string jsonName)
        {
            switch (Common.Language)
            {
                case LanguageEnum.English:
                    _languageKey = "en-US";
                    break;
                case LanguageEnum.Chinese:
                    _languageKey = "zh-CN";
                    break;
                case LanguageEnum.ChineseTraditional:
                    break;
                case LanguageEnum.Japanese:
                    break;
                case LanguageEnum.Russian:
                    break;
                case LanguageEnum.German:
                    break;
                case LanguageEnum.Spanish:
                    break;
                case LanguageEnum.Korean:
                    break;
                case LanguageEnum.Portuguese:
                    break;
                case LanguageEnum.French:
                    break;
                case LanguageEnum.Indonesian:
                    break;
                case LanguageEnum.Polish:
                    break;
                case LanguageEnum.FarsiIran:
                    break;
                case LanguageEnum.ArabicEgypt:
                    break;
                case LanguageEnum.MaxValue:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _list = JsonHelper.ReadOrCreateJson<T>(jsonName);
            
            // Addressables.LoadAssetAsync<>()
        }

        /*public static string Get(string key)
        {
            
        }
        
        public static string Get(int id)
        {
            
        }*/
    }
}