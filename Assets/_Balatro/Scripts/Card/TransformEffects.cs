using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class TransformEffects : MonoBehaviour
{
    [SerializeField] private float _tiltAmplitude;
    [SerializeField] private float _tiltDuration;
    [SerializeField] private float _rotationSpeedMultiplier = 1f;
    [SerializeField] private float _returnDuration = 0.2f;
    [SerializeField] private float _maxRotationAngle = 30f;
    private RectTransform _rectTransform;
    private Vector2 _lastDragPosition;
    private Vector3 _originRotation;
    private Sequence _idleTween;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    void OnDestroy()
    {
        _idleTween?.Kill();
    }

    public void Init()
    {
        StartIdleEffect();
    }

    private void StartIdleEffect()
    {
        // return;
        if (_rectTransform != null)
        {
            _idleTween?.Kill();
            _idleTween = DOTween.Sequence();
            
            _idleTween.Append(_rectTransform.DOLocalRotate(new Vector3(20, _tiltAmplitude * 2, _originRotation.z + (_tiltAmplitude * _originRotation.z < 0 ? -1 : 1)), _tiltDuration).SetEase(Ease.OutSine));
            _idleTween.Append(_rectTransform.DOLocalRotate(new Vector3(-20, -_tiltAmplitude * 2, _originRotation.z + (_tiltAmplitude * _originRotation.z < 0 ? -1 : 1)), _tiltDuration).SetEase(Ease.OutSine))
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void RotateCardWhenMoving(Vector3 position, bool isDragging = false)
    {
        if (_rectTransform == null) return;
        
        if (isDragging)
        {
            var currentDragPosition = position;

            if (_lastDragPosition != Vector2.zero)
            {
                var deltaX = currentDragPosition.x - _lastDragPosition.x;
                var targetAngle = Mathf.Clamp(deltaX * _rotationSpeedMultiplier, -_maxRotationAngle, _maxRotationAngle);
                _rectTransform?.DOLocalRotate(new Vector3(0f, 0f, _originRotation.z +-targetAngle), 0.1f).SetEase(Ease.InSine);
            }

            _lastDragPosition = currentDragPosition;
        }
        else
        {
            _lastDragPosition = Vector2.zero;
            _rectTransform?.DOLocalRotate(Vector3.zero, _returnDuration);
        }
    }
    public void ResetRotation()
    {
        if (_rectTransform == null) return;

        _lastDragPosition = Vector2.zero;
        _rectTransform?.DOLocalRotate(Vector3.zero, _returnDuration);
        StartIdleEffect();
    }

    public void SetupOriginTransform(Vector3 pos, Vector3 rot, bool playIdleEffect = true)
    {
        if (_rectTransform == null) return;

        _rectTransform?.DOAnchorPos(pos, 0.1f).SetEase(Ease.InSine);
        _rectTransform?.DOLocalRotate(rot, 0.1f).SetEase(Ease.InSine);
        _originRotation = rot;

        if (!playIdleEffect) return;
        StartIdleEffect();
    }
}