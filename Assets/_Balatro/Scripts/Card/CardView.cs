using System;
using Common.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Balatro
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] Image _frontImg;
        [SerializeField] Image _backImg;
        [SerializeField] Image _shadow;

        public void SetupView(Sprite sprite, bool active = true)
        {
            _frontImg.gameObject.SetActive(active);
            _backImg.gameObject.SetActive(!active);

            _frontImg.sprite = sprite;
        }

        public void EnableShadow(bool enable)
        {
            var pos = _shadow.rectTransform.anchoredPosition;
            pos.y = enable ? -30 : -10;
            _shadow.rectTransform.anchoredPosition = pos;
        }

        public void EnableBackCard(bool enable)
        {
            if (_backImg.gameObject.activeSelf == enable) return;
            _backImg.gameObject.SetActive(enable);
        }
    }
}
