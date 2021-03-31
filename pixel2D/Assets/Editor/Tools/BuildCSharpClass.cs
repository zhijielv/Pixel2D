using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DG.DemiEditor;
using Framework;
using Sirenix.Utilities;
using UI.ScriptableObjects;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Tools
{
#if UNITY_EDITOR
    public enum ViewScriptType
    {
        View,
        Enum,
    }

    public class BuildCSharpClass
    {
        /// <summary>
        /// 快捷键 Ctrl + Shift + G 生成所有代码
        /// </summary>
        [MenuItem("Assets/FrameWork View/Generate All View #%G", false, -2)]
        public static void GenerateAllUiScriptObject()
        {
            string tmpName = "";
            List<Object> tmpObjList = new List<Object>();
            foreach (string path in Directory.GetFiles(Constants.ViewPrefabDir))
            {
                tmpName = path.FileOrDirectoryName();
                if (tmpName.EndsWith("_View"))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(System.Object)) as GameObject;
                    tmpObjList.Add(prefab);
                }
            }
            GenerateObjList(tmpObjList.ToArray());
        }
        
        [MenuItem("Assets/FrameWork View/Generate Select View", false)]
        public static void GenerateUiScriptObject()
        {
            Object[] views = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            GenerateObjList(views);
        }

        private static void GenerateObjList(Object[] views)
        {
            foreach (var t in views)
            {
                if (!t.name.EndsWith("_View")) continue;
                try
                {
                    GameObject tmpView = t as GameObject;
                    List<string> tmpMember = ExportView(t.name, tmpView);
                    AutoGeneratView(t.name, tmpMember);
                }
                catch (Exception e)
                {
                    Debug.LogError("导出错误 " + e.Message);
                    throw;
                }
            }
        }

        public static List<string> ExportView(string name, GameObject obj)
        {
            PanelScriptableObjectBase asset = ScriptableObject.CreateInstance<PanelScriptableObjectBase>();
            AssetDatabase.CreateAsset(asset, Constants.ScriptableObjectDir + name + ".asset");
            AssetDatabase.SaveAssets();
            asset.PanelObj = obj;
            asset.ResetWidgets();
            return asset.widgetList;
        }

        //实例
        public static void AutoGeneratView(string className, List<string> tmpMember)
        {
            BuildEnum(className, tmpMember);
            BuildClass(className);
        }

        public static void BuildEnum(string className, List<string> tmpMember)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace enumNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            CodeTypeDeclaration myEnum = new CodeTypeDeclaration(className + "_Enum");
            myEnum.IsEnum = true;
            foreach (string s in tmpMember)
            {
                CodeTypeMember member = new CodeMemberField((CodeTypeReference) null, s);
                myEnum.Members.Add(member);
            }

            enumNamespace.Types.Add(myEnum);
            unit.Namespaces.Add(enumNamespace);
            ExportCSharpFile(unit, className, ViewScriptType.Enum);
        }

        public static void BuildClass(string className)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace myNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            myNamespace.Imports.Add(new CodeNamespaceImport("Base"));
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(className);
            myClass.IsClass = true;
            myClass.BaseTypes.Add("ViewBase");
            myClass.TypeAttributes = TypeAttributes.Public;
            myNamespace.Types.Add(myClass);
            unit.Namespaces.Add(myNamespace);
            ExportCSharpFile(unit, className, ViewScriptType.View);
        }


        public static void ExportCSharpFile(CodeCompileUnit unit, string className, ViewScriptType viewScriptType)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            //代码风格:大括号的样式{}
            options.BracingStyle = "C";
            //是否在字段、属性、方法之间添加空白行
            options.BlankLinesBetweenMembers = false;
            //输出文件路径
            string outputFile = Application.dataPath + Constants.ViewScriptDir + className + "/" +
                                viewScriptType.ToString() + "/";
            if (!Directory.Exists(outputFile))
                Directory.CreateDirectory(outputFile);
            string fileName = "tmpCSharpFile.cs";
            switch (viewScriptType)
            {
                case ViewScriptType.Enum:
                    fileName = className + "_Enum.cs";
                    break;
                case ViewScriptType.View:
                    fileName = className + ".cs";
                    break;
            }

            //保存
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile + fileName))
            {
                //为指定的代码文档对象模型(CodeDOM) 编译单元生成代码并将其发送到指定的文本编写器，使用指定的选项。(官方解释)
                //将自定义代码编译器(代码内容)、和代码格式写入到sw中
                provider.GenerateCodeFromCompileUnit(unit, sw, options);
            }
        }
    }
#endif
}