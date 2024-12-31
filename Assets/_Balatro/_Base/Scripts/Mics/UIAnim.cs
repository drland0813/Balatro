using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Common.UI;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject == null) { return null; }

        if (!gameObject.TryGetComponent<T>(out var result))
        {
            result = gameObject.AddComponent<T>();
        }
        return result;
    }
    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
        return GetOrAddComponent<T>(component.gameObject);
    }
}

public class UIAnim : MonoBehaviour
{
    private CanvasGroup _cacheCanvasGroup;

    public RectTransform rectTransform => transform as RectTransform;
    public CanvasGroup canvasGroup => _cacheCanvasGroup ??= this.GetOrAddComponent<CanvasGroup>();

    private Vector3 _originalScale;

    private HashSet<string> _tweenIds = new();

    /// <summary> (Fade in) + (EaseOutBack scale)  </summary>
    public Sequence PlayAppear(float duration = 0.5f, float scaleFactor = 1.5f)
    {
        var tweenId = GetTweenId(nameof(PlayAppear));
        _tweenIds.Add(tweenId);

        if (_originalScale == Vector3.zero) { _originalScale = transform.localScale; }

        canvasGroup.alpha = 0;
        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() => { KillOtherTweensAndActivate(tweenId); })
        .Append(canvasGroup.DOFade(1, duration * 0.5f))
        .Join(transform.DOScale(_originalScale, duration).ChangeStartValue(_originalScale * scaleFactor).SetEase(Ease.OutBack))
        .SetId(tweenId).SetUpdate(true);

        return sequence;
    }

    /// <summary> (Fade out) + (Scale)  </summary>
    public Sequence PlayDisappear(float duration = 0.5f, float scaleFactor = 1.25f)
    {
        var tweenId = GetTweenId(nameof(PlayDisappear));
        _tweenIds.Add(tweenId);

        if (_originalScale == Vector3.zero) { _originalScale = transform.localScale; }

        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() => { KillOtherTweensAndActivate(tweenId); })
        .Append(canvasGroup.DOFade(0, duration * 0.95f))
        .Join(transform.DOScale(_originalScale * scaleFactor, duration))
        .AppendCallback(() => gameObject.SetActive(false))
        .SetId(tweenId).SetUpdate(true);

        return sequence;
    }

    public Sequence FadeIn(float duration = 0.5f, bool useCurrentAlpha = false)
    {
        var tweenId = GetTweenId(nameof(FadeIn));
        _tweenIds.Add(tweenId);

        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() => KillOtherTweensAndActivate(tweenId))
        .Append(canvasGroup.DOFade(1, duration).ChangeStartValue(useCurrentAlpha ? canvasGroup.alpha : 0))
        .SetId(tweenId)
        .SetUpdate(true);

        return sequence;
    }

    public Sequence FadeOut(float duration = 0.5f, bool useCurrentAlpha = true)
    {
        var tweenId = GetTweenId(nameof(FadeOut));
        _tweenIds.Add(tweenId);

        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() => KillOtherTweensAndActivate(tweenId))
        .Append(canvasGroup.DOFade(0, duration).ChangeStartValue(useCurrentAlpha ? canvasGroup.alpha : 1))
        .AppendCallback(() => gameObject.SetActive(false))
        .SetId(tweenId).SetUpdate(true);

        return sequence;
    }

    public Sequence DisableInteractionWhileAnimating(Sequence sequence)
    {
        sequence.OnStart(() => UIManager.Instance.SetUIInteractable(false));
        sequence.OnComplete(() => UIManager.Instance.SetUIInteractable(true));
        return sequence;
    }

    private string GetTweenId(string animName)
    {
        var id = gameObject.GetInstanceID() + animName;
        return id;
    }

    private void KillOtherTweensAndActivate(string currentAnim)
    {
        foreach (var id in _tweenIds)
        {
            if (id == currentAnim) { continue; }
            DOTween.Kill(GetTweenId(id));
        }
        gameObject.SetActive(true);
    }

}