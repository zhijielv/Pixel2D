/*
** Created by fengling
** DateTime:    2021-09-27 10:45:20
** Description: 滚动文字
*/

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Scripts.UI.CustomUI
{
    public static class UiTextExtend
    {
        // 计算字符串在指定text控件中的长度
        public static void RollText(this Text text)
        {
            if (CheckRollText(text) > text.rectTransform.sizeDelta.x)
            {
                TextChange2Roll(text, null);
            }
        }

        public static void RollText(this Text text, float? maxLength)
        {
            if (CheckRollText(text) > text.rectTransform.sizeDelta.x)
            {
                TextChange2Roll(text, maxLength);
            }
        }

        // 获取长度
        private static float CheckRollText(Text text)
        {
            int totalLength = 0;
            string message = text.text;
            Font myFont = text.font;
            myFont.RequestCharactersInTexture(message, text.fontSize, text.fontStyle);
            char[] arr = message.ToCharArray();
            foreach (char c in arr)
            {
                myFont.GetCharacterInfo(c, out var characterInfo, text.fontSize);
                totalLength += characterInfo.advance;
            }
            return totalLength;
        }

        // Text 滚动 todo 拆出来10f的固定长度，换成自定义或者当前字体下的两个空格
        private static void TextChange2Roll(Text text, float? maxLength)
        {
            maxLength ??= CheckRollText(text) + 10f;
            var rectTransform = text.rectTransform;
            rectTransform.sizeDelta = new Vector2((float) maxLength, rectTransform.sizeDelta.y);

            text.name += "_Roll";
            Vector3 text2Pos = new Vector3(rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x,
                rectTransform.anchoredPosition.y, 0);
            GameObject text2 = Object.Instantiate(text.gameObject, text.transform.parent, true);
            text2.GetComponent<RectTransform>().anchoredPosition = text2Pos;
            TextMove(text);
            TextMove(text2.GetComponent<Text>());
        }

        private static void TextMove(Text text)
        {
            RectTransform rectTransform = text.rectTransform;
            rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x - rectTransform.sizeDelta.x,
                rectTransform.sizeDelta.x / 100f).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }
    }
}