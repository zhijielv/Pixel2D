using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DG.DemiEditor;
using Framework;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
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

    public static class BuildCSharpClass
    {
        [MenuItem("Assets/FrameWork View/Set View Value", false, -1)]
        public static void SetViewObj()
        {
            GlobalConfig<UiScriptableObjectsManager>.Instance.isGenerateCode = true;
            AutoGenerateEnd();
        }
        
        /// <summary>
        /// 快捷键 Ctrl + Shift + G 生成所有代码
        /// 会删掉其他代码，尽量不用
        /// </summary>
        // [MenuItem("Assets/FrameWork View/Generate All View #%G", false, -2)]
        public static void GenerateAllUiScriptObject()
        {
            GlobalConfig<UiScriptableObjectsManager>.Instance.isGenerateCode = true;
            GenerateObjList(GetAllViewPrefab().ToArray());
        }

        /// <summary>
        /// 生成选中的View预制体代码
        /// </summary>
        [MenuItem("Assets/FrameWork View/Generate Select View", false)]
        public static void GenerateUiScriptObject()
        {
            Object[] views = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            GlobalConfig<UiScriptableObjectsManager>.Instance.isGenerateCode = true;
            GlobalConfig<UiScriptableObjectsManager>.Instance.selectViews = views;
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
                    SetViewObj();
                }
                catch (Exception e)
                {
                    Debug.LogError("导出错误 " + e.Message);
                    throw;
                }
            }

            Debug.Log("Generate all View Prefab End");
        }
        
        private static List<Object> GetAllViewPrefab(string srcPath = null)
        {
            if (srcPath == null) srcPath = Constants.ViewPrefabDir;
            List<Object> tmpObjList = new List<Object>();
            string tmpName = "";
            foreach (string path in Directory.GetFiles(srcPath, "*.prefab", SearchOption.AllDirectories))
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
            GenerateViewClass(className, tmpMember);
            // 强制刷新unity目录
            AssetDatabase.Refresh();
        }

        private static void GenerateAllView()
        {
            string allViewEnumName = "AllViewEnum";
            Object[] views = GetAllViewPrefab().ToArray();

            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace enumNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            CodeTypeDeclaration allViewEnum = new CodeTypeDeclaration(allViewEnumName) {IsEnum = true};

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
            CodeTypeDeclaration panelEnum = new CodeTypeDeclaration(className + "_Panel") {IsEnum = true};
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

        private static void GenerateViewClass(string className, List<string> tmpMember)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace myNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            myNamespace.Imports.Add(new CodeNamespaceImport("Base"));
            myNamespace.Imports.Add(new CodeNamespaceImport("System"));
            myNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(className)
            {
                IsClass = true, TypeAttributes = TypeAttributes.Public
            };
            myClass.BaseTypes.Add("ViewBase");

            foreach (string s in tmpMember)
            {
                Type type = Constants.GetWidgetTypeByName(s);
                if (type == null)
                {
                    Debug.LogWarning(s + " is not UI");
                    continue;
                }

                CodeTypeMember member = new CodeMemberField(type, s);
                member.Attributes = MemberAttributes.Public;
                myClass.Members.Add(member);
            }

///////////////////////////////////////////////////////////////////////////////////////////
            // 添加override方法
            CodeMemberMethod method = new CodeMemberMethod()
            {
                Name = "GetWidget",
                Attributes = MemberAttributes.Override | MemberAttributes.FamilyAndAssembly,
                ReturnType = new CodeTypeReference(typeof(object)),
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "widgetName"));
            CodeConditionStatement statement = new CodeConditionStatement(
                new CodeVariableReferenceExpression($"!Enum.TryParse(widgetName, true, out {className + "_Widget"} _)"),
                new CodeStatement[]
                {
                    new CodeCommentStatement(
                        "Debug.LogError(gameObject.name + \" has not widget : \" + widgetName);"),
                    new CodeMethodReturnStatement(new CodeSnippetExpression("null"))
                },
                new CodeStatement[]
                {
                    new CodeMethodReturnStatement(
                        new CodeArgumentReferenceExpression("base.GetWidget(widgetName)"))
                });
            method.Statements.Add(statement);
            myClass.Members.Add(method);
///////////////////////////////////////////////////////////////////////////////////////////
            myNamespace.Types.Add(myClass);
            unit.Namespaces.Add(myNamespace);
            ExportCSharpFile(unit, className, ViewScriptType.View);
        }

        private static void ExportCSharpFile(CodeCompileUnit unit, string className, ViewScriptType viewScriptType,
            bool isNeedFolder = true)
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


        #region EndCompile

        // 编译完成后调用
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void AutoGenerateEnd()
        {
            if (!GlobalConfig<UiScriptableObjectsManager>.Instance.isGenerateCode) return;
            // Object[] viewPrefabs = GetAllViewPrefab().ToArray();
            Object[] viewPrefabs = GlobalConfig<UiScriptableObjectsManager>.Instance.selectViews;
            foreach (Object t in viewPrefabs)
            {
                // 添加脚本
                GameObject tmpView = t as GameObject;
                Type viewType = AssemblyUtilities.GetTypeByCachedFullName(Constants.UiNameSpace + t.name);
                if (viewType == null)
                {
                    Debug.LogError($"{tmpView.name} is not Generate");
                    return;
                }

                Debug.LogWarning($"{tmpView.name} is generate, and add component {viewType.FullName}");
                Constants.AddOrGetComponent(tmpView, viewType);
                // 反射赋值
                FieldInfo[] fieldInfos = viewType
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    Type widgetType = Constants.GetWidgetTypeByName(fieldInfo.Name);
                    UiWidgetBase[] children = tmpView.transform.GetComponentsInChildren<UiWidgetBase>();

                    foreach (UiWidgetBase uiWidgetBase in children)
                    {
                        if (!uiWidgetBase.name.EndsWith(fieldInfo.Name)) continue;
                        try
                        {
                            fieldInfo.SetValue(tmpView.GetComponent<ViewBase>(), uiWidgetBase.GetComponent(widgetType));
                        }
                        catch (Exception e)
                        {
                            fieldInfo.SetValue(tmpView.GetComponent<ViewBase>(), uiWidgetBase.gameObject);
                            Debug.Log($"{e}     get GameObject");
                        }
                    }
                }
                EditorUtility.SetDirty(t);
            }

            GlobalConfig<UiScriptableObjectsManager>.Instance.isGenerateCode = false;
        }

        #endregion
    }
#endif
}