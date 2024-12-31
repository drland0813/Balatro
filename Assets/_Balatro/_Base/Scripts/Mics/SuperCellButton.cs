using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class SuperCellButton : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private const float DURATION = 0.04f;
    private const float SCALE_DOWN = 0.94f;

    private Vector3 _originalScale;
    private Vector4 _originalPadding;

    private bool _pressed = false;
    private Selectable _selectable;


    private void Awake()
    {
        _selectable = GetComponent<Selectable>();
        _originalScale = transform.localScale;

        _originalPadding = _selectable.targetGraphic != null ? _selectable.targetGraphic.raycastPadding : Vector4.zero;
    }

    private void EnablePadding(bool enable)
    {
        var graphic = _selectable.targetGraphic;
        if (graphic == null)
        {
            Debug.LogWarning(gameObject.name + " does not have graphic!");
            return;
        }
        if (!enable)
        {
            graphic.raycastPadding = _originalPadding;
            return;
        }

        var widthPadding = graphic.rectTransform.rect.width * (1 - SCALE_DOWN);
        var heightPadding = graphic.rectTransform.rect.height * (1 - SCALE_DOWN);

        var padding = graphic.raycastPadding;
        padding.x -= widthPadding;
        padding.z -= widthPadding;
        padding.y -= heightPadding;
        padding.w -= heightPadding;

        graphic.raycastPadding = padding;
    }

    public void UpdateOriginalScale()
    {
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = _originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_selectable.interactable) { return; }
        _pressed = true;
        EnablePadding(true);
        Scale(false);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerExit(eventData);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_selectable.interactable) { return; }

        _pressed = false;
        Scale(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_pressed) { Scale(true); }

        _pressed = false;
        EnablePadding(false);
    }

    private void Scale(bool up)
    {
        if (this == null)
            return;
        var scaleTo = _originalScale * (up ? 1 : SCALE_DOWN);
        transform.DOKill();
        transform.DOScale(scaleTo, DURATION).SetLink(gameObject).SetUpdate(true);
    }
}