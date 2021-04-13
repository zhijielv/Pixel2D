#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using UnityEngine.UI;


namespace Editor.Tools
{
    public class AnimationClipTool : OdinEditorWindow
    {
        [ReadOnly] [LabelText("角色")] public string avatarName = "";
        [LabelText("每几帧一个图")] public int frame = 5;
        [LabelText("动画名称")] public string animName = "New anim";
        
        //todo 帧率设置
        // private float _frameRate = Common.FrameRate;
        private const float _frameRate = 30;

        [MenuItem("Tools/Sprite2Anim")]
        private static void OpenWindow()
        {
            var window = GetWindow<AnimationClipTool>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 600);
            window.titleContent = new GUIContent("Create Sprite Anim");
        }
        [InlineEditor(InlineEditorModes.LargePreview)]
        [ListDrawerSettings(OnTitleBarGUI = "DrawRefreshButton")]
        [OnValueChanged("SetDefaultName")]
        public List<Sprite> sprites;

        public void SetDefaultName(List<Sprite> sprites)
        {
            if (sprites.Count == 0)
            {
                avatarName = "";
                animName = "";
                return;
            }
            string[] tmpStrs = sprites[0].name.Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries);
            if (tmpStrs.Length <= 1) animName = tmpStrs[0];
            else
            {
                avatarName = tmpStrs[0];
                animName = tmpStrs[0] + "_" + tmpStrs[1];
            }
        }
        public void DrawRefreshButton()
        {
            if(SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
                sprites.Clear();
        }
        
        [HorizontalGroup("Button")][Button(ButtonSizes.Gigantic, Name = "创建动画"), GUIColor(0, 1, 0)]
        public void CreateAnimationClip()
        {
            if (sprites.Count == 0)
            {
                Debug.LogError("未选中贴图");
                return;
            }

            string assetLocation = AssetUtilities.GetAssetLocation(sprites[0]);

            AnimationClip clip = new AnimationClip();
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.type = typeof(Image);
            curveBinding.path = "";
            curveBinding.propertyName = "m_Sprite";
            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Count];
            for (int i = 0; i < sprites.Count; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe {time = i * frame / _frameRate, value = sprites[i]};
            }
            
            clip.frameRate = _frameRate;
            AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyframes);
            
            if (!avatarName.IsNullOrEmpty())
            {
                assetLocation += "/" + avatarName + "_anim";
                Directory.CreateDirectory(assetLocation);
            }
            
            AssetDatabase.CreateAsset(clip, assetLocation + "/" + animName + ".anim");
            AssetDatabase.SaveAssets();
        }

        [HorizontalGroup("Button")]
        [Button(ButtonSizes.Gigantic, Name = "创建AnimatorController"), GUIColor(1, 0, 0)]
        public void CreateAnimatorController()
        {
            
        }
    }
}
#endif