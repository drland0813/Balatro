using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitmapNumber2 : MonoBehaviour
{
    [SerializeField] float _spacing;
    [SerializeField] Sprite[] _numberSprites;
    [SerializeField] SpriteRenderer _signImage;
    [SerializeField] SpriteRenderer _numberImageSample;
    [SerializeField] float _alpha;
    [SerializeField] int _digitCount;

    int _maxNumber;
    List<SpriteRenderer> _numberImages;
    int _currentCount;

    void Awake()
    {
        _maxNumber = (int)Mathf.Pow(10, _digitCount + 1) - 1;

        _numberImages = new List<SpriteRenderer>();
        for (int i = 0; i < _digitCount; i++)
        {
            var image = Instantiate(_numberImageSample, _numberImageSample.transform.parent);
            _numberImages.Add(image);
        }
    }

    void Update()
    {
        var color = Color.white;
        color.a = _alpha;
        _signImage.color = color;
        for (int i = 0; i < _currentCount; i++)
            _numberImages[i].color = color;
    }

    public void SetAlpha(float alpha)
    {
        _alpha = alpha;
    }

    public void ShowNumber(int number)
    {
        if (number > _maxNumber)
            number = _maxNumber;

        var text = number.ToString();
        var w = _signImage.bounds.size.x;
        _signImage.transform.localPosition = new Vector3(w - _signImage.bounds.size.x * 0.5f, 0f);
        _currentCount = 0;
        for (int i = 0; i < _numberImages.Count; i++)
        {
            _numberImages[i].gameObject.SetActive(i < text.Length);
            if (i < text.Length)
            {
                var idx = text[i] - '0';
                _numberImages[i].sprite = _numberSprites[idx];

                w += _numberImages[i].bounds.size.x + _spacing;
                _numberImages[i].transform.localPosition = new Vector3(w - _numberImages[i].bounds.size.x * 0.5f, 0f);
                _currentCount++;
            }
        }
        _numberImages[0].transform.parent.localPosition = new Vector3(-w * 0.5f, 0f);
    }
}
