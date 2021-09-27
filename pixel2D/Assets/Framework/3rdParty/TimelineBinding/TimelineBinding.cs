/*
** Created by fengling
** DateTime:    2021-09-26 10:35:16
** Description: TODO 
*/

using System;
using System.Collections.Generic;
using Framework.Scripts.Constants;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework._3rdParty.TimelineBinding
{
    public class TimelineBinding : MonoBehaviour
    {
        public PlayableDirector PlayableDirector;

        /// <summary>
        /// 加载Timeline文件
        /// </summary>
        /// <param name="playableAsset"></param>
        [Button("创建Timeline")]
        public void CreateTimelineAndBind(PlayableAsset playableAsset)
        {
            JsonHelper.JsonReader(out Dictionary<string, string> playableBindings,
                String.Format(Constants.TimeLineJson, playableAsset.name));
            if (PlayableDirector == null)
                PlayableDirector = new GameObject(playableAsset.name).AddComponent<PlayableDirector>();
            PlayableDirector.playableAsset = playableAsset;
            foreach (KeyValuePair<string, string> keyValuePair in playableBindings)
            {
                foreach (var playableBinding in playableAsset.outputs)
                {
                    if (playableBinding.sourceObject == null) continue;
                    PlayableDirector.SetGenericBinding(playableBinding.sourceObject,
                        GameObject.Find(playableBindings[playableBinding.streamName]));
                }
            }
        }

        /// <summary>
        /// 设置自定义绑定
        /// </summary>
        /// <param name="trackName">轨道名</param>
        /// <param name="targetName">场景中的物体名</param>
        public void SetCustomBinding(string trackName, string targetName)
        {
            foreach (var playableBinding in PlayableDirector.playableAsset.outputs)
            {
                if (playableBinding.streamName != trackName) continue;
                PlayableDirector.SetGenericBinding(playableBinding.sourceObject,
                    GameObject.Find(targetName));
            }
        }
    }
}