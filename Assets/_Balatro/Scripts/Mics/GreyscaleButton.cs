using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreyscaleButton : MonoBehaviour
{
    [SerializeField] protected bool _interactableInGreyscale;
    [SerializeField] protected bool _isGreyText;
    [SerializeField] protected bool _isGreyOutline;
    [SerializeField] protected Color _textColorGrey;
    [SerializeField] protected Color _textColorOutlineGrey;

    static Material s_grayScaleMaterial;
    Button _button;
    bool _isInGreyscale;
    Color _textColorNormal;
    Color _textColorOutlineNormal;

    public void SetButtonEnable(bool enable)
    {
        if (_button != null)
            _button.enabled = _interactableInGreyscale || enable;

        if (!enable && !_isInGreyscale)
        {
            if (s_grayScaleMaterial == null)
            {
                s_grayScaleMaterial = new Material(Shader.Find("Sprites/Grayscale"));
            }
            var images = gameObject.GetComponentsInChildren<Image>(true);
            foreach (var image in images)
                image.material = s_grayScaleMaterial;

            if (_isGreyText)
            {
                var btnText = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (btnText != null)
                {
                    _textColorNormal = btnText.color;
                    btnText.color = _textColorGrey;

                    if (_isGreyOutline)
                    {
                        _textColorOutlineNormal = btnText.outlineColor;
                        btnText.outlineColor = _textColorOutlineGrey;
                    }
                }
            }
        }
        else if (enable && _isInGreyscale)
        {
            var defaultMaterial = Canvas.GetDefaultCanvasMaterial();
            var images = gameObject.GetComponentsInChildren<Image>(true);
            foreach (var image in images)
                image.material = defaultMaterial;

            if (_isGreyText)
            {
                var btnText = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.color = _textColorNormal;

                    if (_isGreyOutline)
                    {
                        btnText.outlineColor = _textColorOutlineNormal;
                    }
                }
            }
        }
        _isInGreyscale = !enable;
    }

    void Awake()
    {
        _button = GetComponent<Button>();
    }
}
