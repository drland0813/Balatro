using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipFullscreenBackground : MonoBehaviour
{
    [SerializeField] float _sizeW = 960f;
    [SerializeField] float _sizeH = 1560f;
    [SerializeField] bool _clipW = true;
    [SerializeField] bool _clipH = true;

    void Awake()
    {
        var rawImage = GetComponent<RawImage>();
        var ratio = (float)Screen.width / Screen.height;
        var height = 1280f;
        var width = 720f;
        if (ratio > 9f / 16f)
        {
            width = height * ratio;
        }
        else
        {
            height = width / ratio;
        }

        var x = 0f;
        var y = 0f;
        var w = 1f;
        var h = 1f;

        if (_clipW)
        {
            rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            x = (_sizeW - width) / _sizeW / 2f;
            w = width / _sizeW;
        }

        if (_clipH)
        {
            rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            y = (_sizeH - height) / _sizeH / 2f;
            h = height / _sizeH;
        }

        rawImage.uvRect = new Rect(x, y, w, h);
    }
}
