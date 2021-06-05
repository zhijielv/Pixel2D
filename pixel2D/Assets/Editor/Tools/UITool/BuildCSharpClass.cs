/*
** Created by fengling
** DateTime:    2021-03-31 13:50:16
** Description: TODO 还有bug，Ctrl+G生成代码之后，未编译完就会赋值，导致新增的参数未赋值成功，需要再次Ctrl+G
*/

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DG.DemiEditor;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using Framework.Scripts.UI.Base;
using Framework.Scripts.UI.CustomUI;
using Framework.Scripts.UI.ScriptableObjects;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SRF;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Tools.UITool
{
#if UNITY_EDITOR
    /// <summary>
    /// MVC模式
    /// View -> C
    /// ViewMember -> V
    /// ScriptableObject -> M
    /// </summary>
    public enum ViewScriptType
    {
        Default,
        ViewMember,
        Module,
        ScriptableObject,
    }

    public static class BuildCSharpClass
    {
        #region MenuItem

        [MenuItem("Assets/FrameWork View/Set View Value", false, -1)]
        public static void SetViewObj()
        {
            Object[] views = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            GlobalConfig<UiBuilderSetting>.Instance.isGenerateCode = true;
            ReflectSetValue(views);
        }

        /// <summary>
        /// 快捷键 Ctrl + G 生成所有代码
        /// </summary>
        [MenuItem("Assets/FrameWork View/Generate All View %G", false, -2)]
        public static void GenerateAllUiScriptObject()
        {
            GenerateAndSave(GetAllViewPrefab().ToArray());
        }

        /// <summary>
        /// 生成选中的View预制体代码
        /// </summary>
        [MenuItem("Assets/FrameWork View/Generate Select View", false)]
        public static void GenerateUiScriptObject()
        {
            GenerateAndSave(Selection.GetFiltered(typeof(Object), SelectionMode.Assets));
        }

        private static void GenerateAndSave(Object[] views)
        {
            GlobalConfig<UiBuilderSetting>.Instance.isGenerateCode = true;
            GenerateObjList(views);
            ReflectSetValue(views);
        }

        #endregion

        #region Generate

        private static void GenerateObjList(Object[] views)
        {
            GenerateAllView2ViewEnum();
            foreach (var t in views)
            {
                if (!t.name.EndsWith("_View")) continue;

                try
                {
                    GameObject tmpView = t as GameObject;
                    // List<string> tmpMember = GetScriptableObjectWidgetList(t.name, tmpView);
                    List<string> tmpMember = RegistWidgets(tmpView.transform);
                    AutoGeneratView(t.name, tmpMember);
                }
                catch (Exception e)
                {
                    Debug.LogError("导出错误 " + e.Message);
                    throw;
                }
            }

            Debug.Log("!!!!!!!!Generate all View Prefab Successful!!!!!!!");
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

        //实例
        private static void AutoGeneratView(string className, List<string> tmpMember)
        {
            GenerateScriptableObjectClass(className);
            GenerateViewWidget(className, tmpMember);
            GenerateViewMember(className, tmpMember);
            GenerateViewClass(className);
            // 强制刷新unity目录
            AssetDatabase.Refresh();
        }

        // 保存所有的UI脚本到AllViewEnum的枚举里
        private static void GenerateAllView2ViewEnum()
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
            ExportCSharpFile(unit, allViewEnumName, ViewScriptType.Default, false);
        }

        // 导出数据
        private static void GenerateScriptableObjectClass(string className)
        {
            // 判断代码是否存在，存在则只修改成员变量
            string outputFile = Application.dataPath + Constants.ViewScriptDir + className + "/" + className +
                                "_ScriptableObject" + ".cs";
            if (File.Exists(outputFile))
                return;
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace myNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            myNamespace.Imports.Add(new CodeNamespaceImport("ScriptableObjects"));
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(className + "_ScriptableObject") {IsClass = true};
            myClass.BaseTypes.Add("PanelScriptableObjectBase");
            myClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
            myNamespace.Types.Add(myClass);
            unit.Namespaces.Add(myNamespace);
            ExportCSharpFile(unit, className, ViewScriptType.ScriptableObject);
        }

        // 导出成员名字Enum
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

        // 导出成员变量
        private static void GenerateViewMember(string className, List<string> tmpMember)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace myNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            myNamespace.Imports.Add(new CodeNamespaceImport("Base"));
            myNamespace.Imports.Add(new CodeNamespaceImport("System"));
            myNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            myNamespace.Imports.Add(new CodeNamespaceImport("Sirenix.OdinInspector"));
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(className)
            {
                IsClass = true, TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed, IsPartial = true,
            };
            myClass.BaseTypes.Add("ViewBase");

            // 添加自定义特性
            CodeAttributeDeclarationCollection collection = new CodeAttributeDeclarationCollection();
            collection.Add(new CodeAttributeDeclaration("FoldoutGroup",
                new CodeAttributeArgument(new CodePrimitiveExpression("Member"))));
            // 添加member
            for (int i = 0; i < tmpMember.Count; i++)
            {
                Type type = Constants.GetWidgetTypeByName(tmpMember[i]);
                if (type == null)
                {
                    Debug.LogWarning(tmpMember[i] + " is not UI");
                    continue;
                }

                CodeTypeMember member = new CodeMemberField(type, tmpMember[i]);
                // 添加注释，member    
                if (i == 0)
                {
                    member.Comments.Add(new CodeCommentStatement("member"));
                }

                member.Attributes = MemberAttributes.Public;
                member.CustomAttributes = collection;
                myClass.Members.Add(member);
            }

            // so
            Type soType = AssemblyUtilities.GetTypeByCachedFullName(
                "Framework.Scripts.UI.ScriptableObjects.PanelScriptableObjectBase");
            CodeTypeMember so = new CodeMemberField(soType, className + "_ScriptableObject");
            so.Attributes = MemberAttributes.Public;
            so.CustomAttributes = collection;
            myClass.Members.Add(so);

///////////////////////////////////////////////////////////////////////////////////////////
            // 添加override方法
            CodeMemberMethod method = new CodeMemberMethod()
            {
                Name = "GetWidget",
                Attributes = MemberAttributes.Override | MemberAttributes.FamilyAndAssembly,
                ReturnType = new CodeTypeReference(typeof(object)),
            };
            if (tmpMember.Count == 0)
                method.Comments.Add(new CodeCommentStatement("member"));

            method.Comments.Add(new CodeCommentStatement("member end"));
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
            ExportCSharpFile(unit, className, ViewScriptType.ViewMember);
        }

        // 导出View类
        private static void GenerateViewClass(string className)
        {
            // 判断代码是否存在，存在则只修改成员变量
            string outputFile = Application.dataPath + Constants.ViewScriptDir + className + "/" + className + ".cs";
            if (File.Exists(outputFile)) return;

            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace myNamespace = new CodeNamespace("Framework.Scripts.UI.View");
            myNamespace.Imports.Add(new CodeNamespaceImport("Base"));
            myNamespace.Imports.Add(new CodeNamespaceImport("System"));
            myNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(className)
            {
                IsClass = true, TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed, IsPartial = true,
            };
            myClass.BaseTypes.Add("ViewBase");
            myNamespace.Types.Add(myClass);
            unit.Namespaces.Add(myNamespace);
            ExportCSharpFile(unit, className, ViewScriptType.Default);
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
            string fileName = viewScriptType switch
            {
                ViewScriptType.Module => className + ".Module.cs",
                ViewScriptType.Default => className + ".cs",
                ViewScriptType.ViewMember => className + ".Member.cs",
                ViewScriptType.ScriptableObject => className + "_ScriptableObject.cs",
                _ => "tmpCSharpFile.cs"
            };

            //保存
            using StreamWriter sw = new StreamWriter(outputFile + fileName);
            //为指定的代码文档对象模型(CodeDOM) 编译单元生成代码并将其发送到指定的文本编写器，使用指定的选项。(官方解释)
            //将自定义代码编译器(代码内容)、和代码格式写入到sw中
            provider.GenerateCodeFromCompileUnit(unit, sw, options);
            provider.Dispose();
        }

        private static List<string> RegistWidgets(Transform obj)
        {
            Transform[] children = obj.GetComponentsInChildren<Transform>();
            List<string> widgetList = new List<string>();
            foreach (Transform child in children)
            {
                if (!CheckName(widgetList, child.name, out UIConfig? uiType)) continue;
                switch (uiType)
                {
                    case UIConfig.Text:
                    case UIConfig.Button:
                    case UIConfig.Image:
                    case UIConfig.InputField:
                    case UIConfig.TouchController:
                        child.gameObject.GetComponentOrAdd<UiWidgetBase>();
                        break;
                    case UIConfig.CustomPanel:
                        child.gameObject.GetComponentOrAdd<CustomPanel>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                widgetList.Add(child.name);
            }

            AssetDatabase.Refresh();
            return widgetList;
        }

        private static bool CheckName(List<string> widgetList, string objName, out UIConfig? uiType)
        {
            string[] nameStrings = objName.Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries);
            if (nameStrings.Length <= 1)
            {
                uiType = null;
                return false;
            }

            string lastName = nameStrings[nameStrings.Length - 1];
            if (widgetList.Contains(lastName))
            {
                Debug.LogError("has same widget Name : " + objName);
                uiType = null;
                return false;
            }

            if (Enum.TryParse(lastName, out UIConfig uiEnum) && !Enum.TryParse(lastName, true, out IgnoreUI ignoreUi))
            {
                uiType = uiEnum;
                return true;
            }

            uiType = null;
            return false;
        }

        #endregion

        #region EndCompile

        // 编译完成后调用
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void AutoGenerateEnd()
        {
            if (!GlobalConfig<UiBuilderSetting>.Instance.isGenerateCode) return;
            GlobalConfig<UiBuilderSetting>.Instance.hasNewUICode = true;
            GlobalConfig<UiScriptableObjectsManager>.Instance.ResetAllViewPrefab();
            GlobalConfig<UiScriptableObjectsManager>.Instance.ResetAllViewSO();
            Object[] viewPrefabs = GlobalConfig<UiScriptableObjectsManager>.Instance.UIPrefabs;
            ReflectSetValue(viewPrefabs);

            // 创建SO
            foreach (var prefab in viewPrefabs)
            {
                GetScriptableObjectWidgetList(prefab as GameObject);
            }
        }

        private static void ReflectSetValue(Object[] viewPrefabs)
        {
            if (!GlobalConfig<UiBuilderSetting>.Instance.hasNewUICode) return;
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
                tmpView.GetComponentOrAdd(viewType);
                // 反射赋值
                FieldInfo[] fieldInfos = viewType
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                // 赋值 view 组件
                foreach (var fieldInfo in fieldInfos)
                {
                    if (fieldInfo.Name.Contains("_ScriptableObject"))
                        continue;

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

            GlobalConfig<UiBuilderSetting>.Instance.isGenerateCode = false;
            EditorUtility.SetDirty(GlobalConfig<UiScriptableObjectsManager>.Instance);
            AddressableAssetsTool.AddAllView2AddressablesGroups();
            Debug.Log("Add All View To AddressablesGroups");
            GlobalConfig<UiBuilderSetting>.Instance.hasNewUICode = false;
        }

        // 创建SO 赋值SO
        private static void GetScriptableObjectWidgetList(GameObject viewObj)
        {
            string name = viewObj.name;
            string path = Constants.ScriptableObjectDir + name + "_Asset.asset";
            // if (File.Exists(path))
            // AssetDatabase.DeleteAsset(path);

            var asset =
                ScriptableObject.CreateInstance(name + "_ScriptableObject");

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Type viewType = AssemblyUtilities.GetTypeByCachedFullName(Constants.UiNameSpace + viewObj.name);
            if (viewType == null)
            {
                Debug.LogError($"{viewObj.name} is not Generate");
                return;
            }

            FieldInfo[] fieldInfos = viewType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (var fieldInfo in fieldInfos)
            {
                if (!fieldInfo.Name.Contains("_ScriptableObject")) continue;
                // 赋值 view 的 so
                fieldInfo.SetValue(viewObj.GetComponent<ViewBase>(), asset);
                break;
            }
        }

        #endregion
    }
#endif
}