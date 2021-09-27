/*
** Created by fengling
** DateTime:    2021-09-27 14:40:02
** Description: TODO 
*/

using System.Collections.Generic;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine.Playables;

namespace Editor.Tools.TimelineTool
{
    public class TimelineTool
    {
        public PlayableDirector playableDirector;
        [ReadOnly] private string jsonPath = "TimeLine/{0}";
        [ShowInInspector, LabelText("绑定信息（轨道名，对象名）")]
        public Dictionary<string, string> PlayableBindings;

        [Button("初始化绑定信息")]
        public void InitPlayableDirector()
        {
            if (playableDirector == null)
                return;
            PlayableBindings = new Dictionary<string, string>();

            foreach (var output in playableDirector.playableAsset.outputs)
            {
                if (output.sourceObject && !PlayableBindings.ContainsKey(output.streamName))
                {
                    PlayableBindings.Add(output.streamName,
                        playableDirector.GetGenericBinding(output.sourceObject).name);
                }
            }
        }

        [Button("导出绑定信息")]
        public void ExportBinding2Json()
        {
            if (playableDirector == null)
                return;
            JsonHelper.JsonWriter(PlayableBindings, string.Format(jsonPath, playableDirector.name));
        }

        [Button]
        public void GetBinding(string bingdingName)
        {
            JsonHelper.JsonReader(out PlayableBindings, bingdingName);
        }
    }
}