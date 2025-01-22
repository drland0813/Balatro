using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Common.Utils;

public class NumberRunText : MonoBehaviour
{
    [SerializeField] float _duration;
    TextMeshProUGUI _numberText;
    int _currentValue;
    int _fromValue;
    int _toValue;
    ActionEaseInterval _actionRun;

    void Awake()
    {
        _numberText = GetComponent<TextMeshProUGUI>();
    }

    public void SetCurrentValue(int value)
    {
        _currentValue = value;
        RefreshNumberText(_currentValue);
    }

    public void SetText(string text)
    {
        _numberText.text = text;
    }

    public void Run(int number)
    {
        _toValue = number;

        bool firstRun = _actionRun == null;
        if (firstRun)
        {
            _actionRun = new ActionEaseInterval
            {
                duration = _duration,
                tweenType = TweenFunc.TweenType.Sine_EaseOut
            };
        }

        if (firstRun || _actionRun.IsFinished)
        {
            _actionRun.Reset();
            StartCoroutine(PlayRun(_duration));
        }
        else
        {
            _actionRun.Reset();
        }
    }

    public void Stop()
    {
        StopAllCoroutines();
        _actionRun = null;
    }

    IEnumerator PlayRun(float duration)
    {
        _fromValue = _currentValue;
        while (!_actionRun.IsFinished)
        {
            var f = _actionRun.Step(Time.deltaTime);
            var value = Mathf.Lerp(_fromValue, _toValue, f);
            var newValue = Mathf.CeilToInt(value);
            if (_currentValue != newValue)
            {
                _currentValue = newValue;
                RefreshNumberText(_currentValue);
            }

            yield return null;
        }

        _currentValue = _toValue;
        RefreshNumberText(_currentValue);
    }

    void RefreshNumberText(int value)
    {
        if (value == 0)
            _numberText.text = "0";
        else
            _numberText.text = value.ToString("#,##");
    }
}
