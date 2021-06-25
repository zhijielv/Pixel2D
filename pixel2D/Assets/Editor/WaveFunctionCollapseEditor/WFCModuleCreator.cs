/*
** Created by fengling
** DateTime:    2021-06-25 13:30:34
** Description: TODO 
*/

using System.Collections.Generic;
using Framework._3rdParty.WaveFunctionCollapse;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;

namespace Editor.WaveFunctionCollapseEditor
{
    public class WFCModuleCreator
    {
        [ShowInInspector]
        public List<ModuleData> ModuleDatas;

        public WFCModuleCreator()
        {
            // ModuleDatas = new List<ModuleData>();
            // LoadJson();
        }

        [Button]
        [ButtonGroup("JsonBtn")]
        public void LoadJson()
        { 
            ModuleDatas = JsonHelper.JsonReader<ModuleData>("WFCModule");
        }
        
        [Button]
        [ButtonGroup("JsonBtn")]
        public void CreateJson()
        {
            JsonHelper.JsonWriter(ModuleDatas, "WFCModule");
        }
    }
}