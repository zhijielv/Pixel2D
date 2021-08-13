﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
#if UNITY_EDITOR
using DG.DemiEditor;
using UnityEditor;
#endif
using Framework.Scripts.Manager;
using Newtonsoft.Json;
using UnityEngine;

namespace Framework.Scripts.Utils
{
    public class JsonHelper
    {
        /// <summary>
        /// 运行时直接读取资源包加载json
        /// 编辑器下读取项目路径，不存在的json需要导出新json并存入包
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> JsonReader<T>(string jsonPath)
        {
            TextAsset jsonFile = null;

            if (Application.isPlaying)
            {
                jsonFile = AddressableManager.Instance.LoadAsset<TextAsset>(jsonPath);
            }
            else
            {
                jsonPath = Constants.Constants.JsonFoldreDir + jsonPath + ".json";
#if UNITY_EDITOR
                jsonFile = !File.Exists(jsonPath)
                    ? new TextAsset("[]")
                    : AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
#endif
            }

            List<T> t = JsonConvert.DeserializeObject<List<T>>(jsonFile.text);
            return t;
        }

#if UNITY_EDITOR
        public static string JsonWriter<T>(List<T> list, string jsonPath)
        {
            jsonPath = Constants.Constants.JsonFoldreDir + jsonPath + ".json";

            if (!Directory.Exists(jsonPath.Parent()))
            {
                Debug.Log($"创建 {jsonPath.Parent()}");
                Directory.CreateDirectory(jsonPath.Parent());
                FileStream fileStream = File.Create(jsonPath);
                fileStream.Close();
            }

            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            StreamWriter streamWriter = new StreamWriter(jsonPath) {AutoFlush = true};
            streamWriter.Write(json);
            streamWriter.Close();
            AssetDatabase.Refresh();
            return json;
        }


        /// <summary>
        /// 格式化读取某种类型的Json列表
        /// </summary>
        /// <param name="jsonName">json文件名（不带后缀）</param>
        /// <typeparam name="T">要获取的类</typeparam>
        public static List<T> ReadOrCreateJson<T>(string jsonName) where T : class, new()
        {
            string json;
            if (Application.isPlaying)
            {
                TextAsset textAsset = AddressableManager.Instance.LoadAsset<TextAsset>(jsonName);
                json = textAsset.text;
                return JsonConvert.DeserializeObject<List<T>>(json);
            }

            string jsonPath = $"Assets/Framework/Json/{jsonName}.json";
            if (!Directory.Exists(jsonPath.Parent()))
            {
                Debug.Log($"创建 {jsonName}.json at " + jsonPath);
                Directory.CreateDirectory(jsonPath.Parent());
                FileStream fileStream = File.Create(jsonPath);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write("[]");
                streamWriter.Close();
                fileStream.Close();
            }

            var streamReader = new StreamReader(jsonPath);
            json = streamReader.ReadToEnd();
            streamReader.Close();
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
#endif
        /// <summary>
        /// 格式化Json类
        /// </summary>
        /// <param name="jsonName">Assets/Framework/Json/下的json文件（不带后缀）</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static T GetJsonClass<T>(string jsonName) where T : class
        {
            string jsonPath = $"Assets/Framework/Json/{jsonName}.json";
            if (!File.Exists(jsonPath))
            {
                Debug.LogError("没有json文件");
                return null;
            }

            var streamReader = new StreamReader(jsonPath);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    /// <summary>
    /// 解决字典类型有enum字段时的拆箱装箱GC问题
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumComparer<T> : IEqualityComparer<T> where T : Enum
    {
        public bool Equals(T first, T second)
        {
            var firstParam = Expression.Parameter(typeof(T), "first");
            var secondParam = Expression.Parameter(typeof(T), "second");
            var equalExpression = Expression.Equal(firstParam, secondParam);
            return Expression.Lambda<Func<T, T, bool>>
                (equalExpression, new[] {firstParam, secondParam}).Compile().Invoke(first, second);
        }

        public int GetHashCode(T instance)
        {
            var parameter = Expression.Parameter(typeof(T), "instance");
            var convertExpression = Expression.Convert(parameter, typeof(int));
            return Expression.Lambda<Func<T, int>>
                (convertExpression, new[] {parameter}).Compile().Invoke(instance);
        }
    }
}