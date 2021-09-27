/*
** Created by fengling
** DateTime:    2021-09-26 10:35:16
** Description: TODO 
*/

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework._3rdParty.TimelineBinding
{
    public class TimelineBinding : MonoBehaviour
    {
        public PlayableDirector playableDirector;
        [ShowInInspector]
        public Dictionary<string, string> playableBindings;

        [Button("创建Timeline")]
        public void CreateTimelineAndBind(PlayableAsset playableAsset)
        {
            PlayableDirector director = new GameObject(playableAsset.name).AddComponent<PlayableDirector>();
            director.playableAsset = playableAsset;
            foreach (KeyValuePair<string, string> keyValuePair in playableBindings)
            {
                foreach (var playableBinding in playableAsset.outputs)
                {
                    if(playableBinding.sourceObject == null) continue;
                    director.SetGenericBinding(playableBinding.sourceObject,
                        GameObject.Find(playableBindings[playableBinding.streamName]));
                }
            }
        }
    }
}