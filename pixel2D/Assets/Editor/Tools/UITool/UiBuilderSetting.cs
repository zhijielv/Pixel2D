/*
** Created by fengling
** DateTime:    2021-05-13 13:50:16
** Description: ui生成器相关设置
*/

using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Object = System.Object;

namespace Editor.Tools.UITool
{
    // [HideMonoScript]
    [GlobalConfig("Editor/Tools/UITool/")]
    public class UiBuilderSetting : GlobalConfig<UiBuilderSetting>
    {
        public bool isGenerateCode = false;
        public bool hasNewUICode = false;
        [ShowInInspector]
        [ReadOnly] public Object[] selectViews;

        [Button("Generate All UI", ButtonSizes.Large)]
        public void GenerateAllUiScriptObject()
        {
            BuildCSharpClass.GenerateAllUiScriptObject();
        }
    }
}