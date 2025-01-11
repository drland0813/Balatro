using System;
using System.Collections;
using System.Collections.Generic;
using Common.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Balatro
{
    public interface ICardLogic
    {
        
    }

    public enum CardType
    {
        Spades = 1,
        Clubs,
        Diamonds, 
        Hearts 
    }
    
    [Serializable]
    public class CardData
    {
        public string ID;
        public int Value;
        public int Chip;
        public int Type;
        public string Description;

        public CardData(string id, int value, int chip, int type, string description)
        {
            ID = id;
            Value = value;
            Chip = chip;
            Type = type;
            Description = description;
        }
    }

    public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] protected CardView _view;
        [SerializeField] protected CardData _data;
        
        protected RectTransform _rectTransform;
        protected Vector2 _originalPosition;
        protected CardSlot _cardSlot;
        protected TransformEffects _transfromEffect;

        
        public bool IsChosen;
        protected bool _isDragging;

        public Action<Card> OnBeginDragAction;
        public Action OnDragAction;
        public Action OnEndDragAction;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _transfromEffect = GetComponent<TransformEffects>();
            _cardSlot = transform.parent.GetComponent<CardSlot>();
        }

        public void Init(CardData data)
        {
            _data = data;

        }

        public void SetupView(Sprite sprite)
        {
            _view.SetupView(sprite);
        }
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            SetCardOnTop(true);

            OnBeginDragAction?.Invoke(this);
            _isDragging = true;
            _originalPosition = _rectTransform.anchoredPosition;
        }

        private void SetCardOnTop(bool enable)
        {
            var graphicRaycaster = transform.GetOrAddComponent<GraphicRaycaster>();
            var canvas = transform.gameObject.GetComponent<Canvas>();
            if (enable)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 10;
                canvas.sortingLayerName = "UI";
            }
            else
            {
                Destroy(graphicRaycaster);
                Destroy(canvas);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragAction?.Invoke();
            if (IsPointerInsideScreen(eventData.position))
            {
                _view.EnableShadow(true);
                _transfromEffect.RotateCardWhenMoving(eventData.position, true);
                var worldPosition = UIManager.Instance.CameraUI.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y,UIManager.Instance.CameraUI.nearClipPlane));
                worldPosition.z = 0;
                _rectTransform.position = worldPosition;
            }
            else
            {
                ResetTransform();
                _view.EnableShadow(false);
                // _transfromEffect.ResetRotation();
            }
        }


        private void ResetTransform()
        {
            SetCardOnTop(false);
            var newPos = _cardSlot.CardPosition;
            if (IsChosen)
            {
                newPos.y += 40;
            }
            _rectTransform.DOAnchorPos(newPos, 0.2f).SetEase(Ease.OutBack)
                .OnComplete((() =>
                {
                    _transfromEffect.SetupOriginTransform(newPos, _cardSlot.CardLocalRotation);
                }));

            var sequence = DOTween.Sequence();
            sequence.Append(_rectTransform.DOScale(1f, 0.025f));
        }
        
        private bool IsPointerInsideScreen(Vector2 pointerPosition)
        {
            return pointerPosition.x >= 0 && pointerPosition.x <= Screen.width &&
                   pointerPosition.y >= 0 && pointerPosition.y <= Screen.height;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragAction?.Invoke();
            ResetTransform();
            _view.EnableShadow(false);
            _isDragging = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(_rectTransform.DOScale(1.2f, 0.025f));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDragging) return;

            IsChosen = !IsChosen;
            var sequence = DOTween.Sequence();

            var y = _rectTransform.anchoredPosition.y;
            y += IsChosen ? 40 : -40;

            sequence.Append(_rectTransform.DOScale(1.2f, 0.025f))
                .Append(_rectTransform.DOAnchorPosY(y, 0.1f))
                .Append(_rectTransform.DOScale(1f, 0.025f));
        }

        public void SetupOriginTransform(Vector3 pos, Vector3 rot)
        {
            _transfromEffect.SetupOriginTransform(pos, rot);
        }

        public void SetCardSlot(CardSlot cardSlot)
        {
            _cardSlot = cardSlot;
            transform.SetParent(cardSlot.transform);
        }

        public void RotateCardWhenMoving(Vector3 pos)
        {
            _transfromEffect.RotateCardWhenMoving(pos, true);
        }
    }
}
