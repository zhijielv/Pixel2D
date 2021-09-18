/* 
** Created by lvzj
** DateTime: 2021-09-21 16:24:51
** Description: ui特效跟随fillamount位置
*/

using System;
using UnityEngine;
using UnityEngine.UI;

// [ExecuteAlways]
public class UIFilledExtend : MonoBehaviour
{
    public GameObject obj;

    // 是否翻转图片
    public bool flip;

    // 映射到图片边缘
    public bool onSide;

    // 距离旋转点偏移量
    [Range(0, 1)]
    public float offset = 0.5f;

    Vector2[] _points = new Vector2[3];

    private Image _image;

    private RectTransform _rectTransform;

    // 旋转
    private Vector3 _tempRotate = Vector3.zero;

    // 测试点位
    public Image[] pointImages = new Image[3];

    private bool CheckFilledExtend()
    {
        if (obj == null) return false;
        _image = transform.GetComponent<Image>();
        return _image.type == Image.Type.Filled;
    }

    private void Start()
    {
        _image = transform.GetComponent<Image>();
        _rectTransform = transform.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (CheckFilledExtend())
        {
            GetPosition();
            SetObjPosition();
        }
    }
    
    private void GetPosition()
    {
        _image = transform.GetComponent<Image>();
        _rectTransform = transform.GetComponent<RectTransform>();
        float fillAmount = _image.fillAmount;
        bool fillClockwise = _image.fillClockwise;

        int fillOrigin = _image.fillOrigin;
        float amount = Mathf.Pow(-1, fillOrigin);
        float pos = fillAmount * amount + fillOrigin;

        // 原点
        float point0X;
        float point0Y;

        // 目标点
        float tempPointX;
        float tempPointY;

        float deltaX;
        float deltaY;
        float absX;
        float absY;
        switch (_image.fillMethod)
        {
            case Image.FillMethod.Horizontal:
                point0X = tempPointX = pos;
                point0Y = 0;
                tempPointY = 1f;
                _tempRotate = Vector3.zero;
                break;
            case Image.FillMethod.Vertical:
                point0X = 0;
                point0Y = tempPointY = pos;
                tempPointX = 1f;
                _tempRotate = Vector3.zero;
                break;
            case Image.FillMethod.Radial90:
                point0X = fillOrigin < 2 ? 0 : 1;
                point0Y = fillOrigin < 2 ? fillOrigin % 2 : (fillOrigin - 1) % 2;
                float rad;
                bool isTop = fillOrigin % 3 != 0;
                fillClockwise = isTop ? !fillClockwise : fillClockwise;
                rad = Trans01(fillAmount, fillClockwise == fillOrigin < 2) * 90f * Mathf.Deg2Rad;
                tempPointX = Trans01(Mathf.Cos(rad), fillOrigin >= 2);
                tempPointY = Trans01(Mathf.Sin(rad), isTop);

                if (onSide)
                {
                    deltaX = tempPointX - point0X;
                    deltaY = tempPointY - point0Y;
                    absX = Mathf.Abs(deltaX);
                    absY = Mathf.Abs(deltaY);
                    if (absX > absY)
                    {
                        tempPointY = deltaY * (1.0f / absX) + point0Y;
                        tempPointX = fillOrigin >= 2 ? 0 : 1;
                    }
                    else
                    {
                        tempPointX = deltaX * (1.0f / absY) + point0X;
                        tempPointY = isTop ? 0 : 1;
                    }
                }

                break;
            case Image.FillMethod.Radial180:
                point0X = Mathf.Abs((fillOrigin - 1) / 2.0f);
                point0Y = fillOrigin % 2 == 0 ? fillOrigin / 2.0f : 0.5f;

                //bottom
                if (fillOrigin == 0)
                {
                    tempPointX = Mathf.Cos(Trans01(fillAmount, fillClockwise) * 180f * Mathf.Deg2Rad) / 2.0f + 0.5f;
                    tempPointY = Mathf.Sin(fillAmount * 180f * Mathf.Deg2Rad);
                }

                //top
                else if (fillOrigin == 2)
                {
                    tempPointX = Mathf.Cos(Trans01(fillAmount, !fillClockwise) * 180f * Mathf.Deg2Rad) / 2.0f + 0.5f;
                    tempPointY = 1 - Mathf.Sin(fillAmount * 180f * Mathf.Deg2Rad);
                }
                //left
                else if (fillOrigin == 1)
                {
                    tempPointX = Mathf.Sin(fillAmount * 180f * Mathf.Deg2Rad);
                    tempPointY = Mathf.Cos(Trans01(fillAmount, !fillClockwise) * 180f * Mathf.Deg2Rad) / 2.0f + 0.5f;
                }
                // right fillOrigin == 3
                else
                {
                    tempPointX = 1 - Mathf.Sin(fillAmount * 180f * Mathf.Deg2Rad);
                    tempPointY = Mathf.Cos(Trans01(fillAmount, fillClockwise) * 180f * Mathf.Deg2Rad) / 2.0f + 0.5f;
                }

                if (onSide)
                {
                    deltaX = (tempPointX - point0X) / (fillOrigin % 2 == 1 ? 2.0f : 1.0f);
                    deltaY = (tempPointY - point0Y) / (fillOrigin % 2 == 0 ? 2.0f : 1.0f);
                    absX = Mathf.Abs(deltaX);
                    absY = Mathf.Abs(deltaY);
                    if (absX > absY)
                    {
                        tempPointY = deltaY / (fillOrigin % 2 == 1 ? 2.0f : 1.0f) * (1.0f / absX) + point0Y;
                        tempPointX = deltaX >= 0 ? 1 : 0;
                    }
                    else
                    {
                        tempPointX = deltaX / (fillOrigin % 2 == 0 ? 2.0f : 1.0f) * (1.0f / absY) + point0X;
                        tempPointY = deltaY >= 0 ? 1 : 0;
                    }
                }

                break;
            case Image.FillMethod.Radial360:
                point0X = point0Y = 0.5f;

                // right
                if (fillOrigin == 1)
                {
                    tempPointX = Mathf.Cos(Trans01(fillAmount, fillClockwise) * 360f * Mathf.Deg2Rad) / 2.0f + 0.5f;
                    tempPointY = Mathf.Sin(Trans01(fillAmount, fillClockwise) * 360f * Mathf.Deg2Rad) / 2.0f + 0.5f;
                }
                // left
                else if (fillOrigin == 3)
                {
                    tempPointX = 1 - Mathf.Cos(Trans01(fillAmount, fillClockwise) * 360f * Mathf.Deg2Rad) / 2.0f - 0.5f;
                    tempPointY = Mathf.Sin(Trans01(fillAmount, !fillClockwise) * 360f * Mathf.Deg2Rad) / 2.0f + 0.5f;
                }
                // bottom
                else if (fillOrigin == 0)
                {
                    tempPointX = Mathf.Sin(Trans01(fillAmount, fillClockwise) * 360f * Mathf.Deg2Rad) / 2.0f + 0.5f;
                    tempPointY = 1 - Mathf.Cos(Trans01(fillAmount, !fillClockwise) * 360f * Mathf.Deg2Rad) / 2.0f -
                                 0.5f;
                }
                // top
                else
                {
                    tempPointX = Mathf.Sin(Trans01(fillAmount, !fillClockwise) * 360f * Mathf.Deg2Rad) / 2.0f + 0.5f;
                    tempPointY = Mathf.Cos(Trans01(fillAmount, fillClockwise) * 360f * Mathf.Deg2Rad) / 2.0f + 0.5f;
                }

                if (onSide)
                {
                    deltaX = tempPointX - point0X;
                    deltaY = tempPointY - point0Y;
                    absX = Mathf.Abs(deltaX);
                    absY = Mathf.Abs(deltaY);
                    if (absX > absY)
                    {
                        tempPointY = deltaY * (0.5f / absX) + point0Y;
                        tempPointX = deltaX >= 0 ? 1 : 0;
                    }
                    else
                    {
                        tempPointX = deltaX * (0.5f / absY) + point0X;
                        tempPointY = deltaY >= 0 ? 1 : 0;
                    }
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // 旋转
        _tempRotate = new Vector3(0, 0,
            Mathf.Atan((tempPointY - point0Y) / (tempPointX - point0X)) * Mathf.Rad2Deg + (flip ? (-90) : 90));
        // [0,180]->[0,360]
        if (tempPointX - point0X < 0)
        {
            _tempRotate.z += 180;
        }

        // offset
        Vector2 vector2 = new Vector2(tempPointX - point0X, tempPointY - point0Y);
        vector2 *= offset;
        tempPointX = vector2.x + point0X;
        tempPointY = vector2.y + point0Y;

        // 设置点位
        _points = new[]
        {
            new Vector2(point0X, point0Y),
            new Vector2((point0X + tempPointX) / 2, (point0Y + tempPointY) / 2),
            new Vector2(tempPointX, tempPointY),
        };

        // 对应到ui点上(测试点位)
        for (int i = 0; i < pointImages.Length; i++)
        {
            pointImages[i].rectTransform.anchoredPosition = new Vector2(
                (_points[i].x - 0.5f) * _rectTransform.sizeDelta.x + _rectTransform.anchoredPosition.x,
                (_points[i].y - 0.5f) * _rectTransform.sizeDelta.y + _rectTransform.anchoredPosition.y);
            pointImages[i].transform.rotation = Quaternion.Euler(_tempRotate);
        }
    }

    private void SetObjPosition()
    {
        if (!obj) return;
        var sizeDelta = _rectTransform.sizeDelta;
        var position = _rectTransform.transform.localPosition;
        obj.GetComponent<RectTransform>().transform.localPosition = new Vector2(
            (_points[2].x - 0.5f) * sizeDelta.x + position.x,
            (_points[2].y - 0.5f) * sizeDelta.y + position.y);
        obj.GetComponent<RectTransform>().transform.rotation = Quaternion.Euler(_tempRotate);
    }

    private float Trans01(float value, bool needTrans)
    {
        if (needTrans)
            return 1 - value;
        return value;
    }
}