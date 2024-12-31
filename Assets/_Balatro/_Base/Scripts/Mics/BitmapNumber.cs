using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BitmapNumber : MonoBehaviour
{
    [SerializeField] Sprite[] _numberSprites;
    [SerializeField] Image _numberImageSample;

    [SerializeField] int _digitCount;

    int _maxNumber;
    List<Image> _numberImages;

    void Awake()
    {
        _maxNumber = (int)Mathf.Pow(10, _digitCount + 1) - 1;

        _numberImages = new List<Image>();
        for (int i = 0; i < _digitCount; i++)
        {
            var image = Instantiate(_numberImageSample, _numberImageSample.transform.parent);
            _numberImages.Add(image);
        }
    }

    public void ShowNumber(int number)
    {
        if (number > _maxNumber)
            number = _maxNumber;

        var text = number.ToString();
        for (int i = 0; i < _numberImages.Count; i++)
        {
            _numberImages[i].gameObject.SetActive(i < text.Length);
            if (i < text.Length)
            {
                var idx = text[i] - '0';
                _numberImages[i].sprite = _numberSprites[idx];
                _numberImages[i].SetNativeSize();
            }
        }
    }
}
