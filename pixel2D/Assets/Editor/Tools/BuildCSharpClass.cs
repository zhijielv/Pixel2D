using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DG.DemiEditor;
using Framework;
using Framework.Scripts.UI.Base;
using Framework.Scripts.UI.ScriptableObjects;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Tools
{
#if UNITY_EDITOR
    public enum ViewScriptType
    {
        View,
        Module,
        ScriptableObject,
    }

    public class BuildCSharpClass
    {
        /// <summary>
        /// 快捷键 Ctrl + Shift + G 生成所有代码
        /// </summary>
        [MenuItem("Assets/FrameWork View/Generate All View #%G", false, -2)]
        public static void GenerateAllUiScriptObject()
        {
            
            GenerateObjList(GetAllViewPrefab().ToArray());
        }
        
        [MenuItem("Assets/FrameWork View/Generate Select View", false)]
        public static void GenerateUiScriptObject()
        {
            Object[] views = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            GenerateObjList(views);
        }

        private static void GenerateObjList(Object[] views)
        {
            GenerateAllView();
            foreach (var t in views)
            {
                if (!t.name.EndsWith("_View")) continue;
                
                try
                {
                    GameObject tmpView = t as GameObject;
                    List<string> tmpMember = GetScriptableObjectWidgetList(t.name, tmpView);
                    AutoGeneratView(t.name, tmpMember);
                    // 强制刷新unity目录
                    AssetDatabase.Refresh();
                    string tname = Constants.UiNameSpace + t.name;
                    Type type = AssemblyUtilities.GetTypeByCachedFullName(tname);
                    if (type == null)
                    {
                        Debug.Log(Constants.UiNameSpace + t.name);
                        Debug.LogError($"{tmpView.name} is not Generate");
                    }
                    else
                    {
                        Debug.LogWarning($"{tmpView.name} is generate, and add component {type.FullName}");
                    }
                    ViewBase viewBase = (ViewBase) Constants.AddOrGetComponent(tmpView, type);
                    viewBase.ResetData();
                }
                catch (Exception e)
                {
                    Debug.LogError("导出错误 " + e.Message);
                    throw;
                }
            }

            Debug.Log("Generate all View Prefab End");
        }

        private static List<Object> GetAllViewPrefab()
        {
            List<Object> tmpObjList = new List<Object>();
            string tmpName = "";
            foreach (string path in Directory.GetFiles(Constants.ViewPrefabDir))
            {
                tmpName = path.FileOrDirectoryName();
                if (tmpName.EndsWith("_View"))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(System.Object)) as GameObject;
                    tmpObjList.Add(prefab);
                }
            }

            return tmpObjList;
        }

        private static List<string> GetScriptableObjectWidgetList(string name, GameObject obj)
        {
            PanelScriptableObjectBase asset = ScriptableObject.CreateInstance<PanelScriptableObjectBase>();
            AssetDatabase.CreateAsset(asset, Constants.ScriptableObjectDir + name + ".asset");
            AssetDatabase.SaveAssets();
            asset.PanelObj = obj;
            asset.ResetWidgets();
            return asset.widgetList;
        }
        
        //实例
        private static void AutoGeneratView(string className, List<string> tmpMember)
        {
            GenerateScriptableObjectClass(className);
            GenerateViewWidget(className, tmpMember);
            GenerateViewClass(className);
        }

        private static void GenerateAllView()
        {
            string allViewEnumName = "AllViewEnum";
            Object[] views = GetAllViewPrefab().ToArray();
            
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace enumNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            CodeTypeDeclaration allViewEnum = new CodeTypeDeclaration(allViewEnumName){IsEnum = true};
            
            foreach (Object view in views)
            {
                if (!view.name.EndsWith("_View")) continue;
                CodeTypeMember member = new CodeMemberField((CodeTypeReference) null, view.name);
                allViewEnum.Members.Add(member);
            }
            CodeTypeMember maxMember = new CodeMemberField((CodeTypeReference) null, "MaxValue");
            allViewEnum.Members.Add(maxMember);
            
            enumNamespace.Types.Add(allViewEnum);
            unit.Namespaces.Add(enumNamespace);
            ExportCSharpFile(unit, allViewEnumName, ViewScriptType.Module, false);
        }

        private static void GenerateScriptableObjectClass(string className)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace myNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            myNamespace.Imports.Add(new CodeNamespaceImport("ScriptableObjects"));
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(className + "_ScriptableObject") {IsClass = true};
            myClass.BaseTypes.Add("PanelScriptableObjectBase");
            myClass.TypeAttributes = TypeAttributes.Public;
            myNamespace.Types.Add(myClass);
            unit.Namespaces.Add(myNamespace);
            ExportCSharpFile(unit, className, ViewScriptType.ScriptableObject);
        }
        private static void GenerateViewWidget(string className, List<string> tmpMember)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace enumNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            CodeTypeDeclaration panelEnum = new CodeTypeDeclaration(className + "_Panel"){IsEnum = true};
            CodeTypeDeclaration widgetEnum = new CodeTypeDeclaration(className + "_Widget") {IsEnum = true};
            foreach (string s in tmpMember)
            {
                CodeTypeMember member = new CodeMemberField((CodeTypeReference) null, s);
                if (s.EndsWith("_Panel")) panelEnum.Members.Add(member);
                else widgetEnum.Members.Add(member);
            }
            enumNamespace.Types.Add(panelEnum);
            enumNamespace.Types.Add(widgetEnum);
            unit.Namespaces.Add(enumNamespace);
            ExportCSharpFile(unit, className, ViewScriptType.Module);
        }

        private static void GenerateViewClass(string className)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace myNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            myNamespace.Imports.Add(new CodeNamespaceImport("Base"));
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(className) {IsClass = true};
            myClass.BaseTypes.Add("ViewBase");
            myClass.TypeAttributes = TypeAttributes.Public;
            myNamespace.Types.Add(myClass);
            unit.Namespaces.Add(myNamespace);
            ExportCSharpFile(unit, className, ViewScriptType.View);
        }
        
        private static void ExportCSharpFile(CodeCompileUnit unit, string className, ViewScriptType viewScriptType, bool isNeedFolder = true)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            //代码风格:大括号的样式{}
            options.BracingStyle = "C";
            //是否在字段、属性、方法之间添加空白行
            options.BlankLinesBetweenMembers = false;
            //输出文件路径
            string outputFile = Application.dataPath + Constants.ViewScriptDir;
            if (isNeedFolder) outputFile += className + "/";
            else outputFile += "/";
            if (!Directory.Exists(outputFile))
                Directory.CreateDirectory(outputFile);
            string fileName = "tmpCSharpFile.cs";
            switch (viewScriptType)
            {
                case ViewScriptType.Module:
                    fileName = className + "_Module.cs";
                    break;
                case ViewScriptType.View:
                    fileName = className + ".cs";
                    break;
                case ViewScriptType.ScriptableObject:
                    fileName = className + "_ScriptableObject.cs";
                    break;
            }
            
            //保存
            using (StreamWriter sw = new StreamWriter(outputFile + fileName))
            {
                //为指定的代码文档对象模型(CodeDOM) 编译单元生成代码并将其发送到指定的文本编写器，使用指定的选项。(官方解释)
                //将自定义代码编译器(代码内容)、和代码格式写入到sw中
                provider.GenerateCodeFromCompileUnit(unit, sw, options);
            }
        }

        // todo 改为导出view代码时调用
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void Test()
        {
            Object[] viewPrefabs = GetAllViewPrefab().ToArray();
            Type[] viewScripts = new Type[viewPrefabs.Length];
            for (int i = 0; i < viewPrefabs.Length; i++)
            {
                viewScripts[i] = AssemblyUtilities.GetTypeByCachedFullName(Constants.UiNameSpace + viewPrefabs[i].name);
                FieldInfo[] fieldInfos = viewScripts[i].GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    string[] tmpStrs = fieldInfo.Name.Split(new []{"_"}, StringSplitOptions.RemoveEmptyEntries);
                    string typeName = tmpStrs[tmpStrs.Length - 1];
                    Type fieldType = AssemblyUtilities.GetTypeByCachedFullName("UnityEngine.UI." + typeName);
                    GameObject viewPrefab = viewPrefabs[i] as GameObject;
                    UiWidgetBase[] children = viewPrefab.transform.GetComponentsInChildren<UiWidgetBase>();
                    // if (fieldInfo.GetValue(fieldType) != null)
                    // {
                    //     Debug.Log(fieldInfo.GetValue(fieldType));
                    //     continue;
                    // }
                    // todo 把获取到的成员通过反射赋值
                    foreach (UiWidgetBase uiWidgetBase in children)
                    {
                        if(!uiWidgetBase.name.EndsWith(fieldInfo.Name)) continue;
                        // Debug.Log($"{fieldType}           {uiWidgetBase.GetComponent(fieldType).GetType()}");
                        // Debug.Log(fieldType);
                        // Debug.Log(uiWidgetBase.GetComponent(fieldType));
                        
                        // fieldInfo.SetValue(fieldType, uiWidgetBase.GetComponent(fieldType));
                        Debug.Log(fieldType.ToString());

                        Debug.Log(viewScripts[i].GetProperty(fieldType.ToString()));
                        Type propertyType = viewScripts[i].GetProperty(fieldType.ToString()).PropertyType;
                        Debug.Log(propertyType);
                        object v = Convert.ChangeType(uiWidgetBase.GetComponent(fieldType), propertyType);
                        viewScripts[i].GetProperty(fieldType.ToString()).SetValue(v, uiWidgetBase.GetComponent(fieldType));
                    }
                }
            }
        }
    }
#endif
}