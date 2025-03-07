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

        public CardData()
        {
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
        public Action<Card> OnClick;

        public bool CanInteract = true;

        private Sequence _moveTween;
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
            if (!CanInteract) return;
            
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
            if (!CanInteract) return;

            OnDragAction?.Invoke();
            if (IsPointerInsideScreen(eventData.position))
            {
                _view.EnableShadow(true);
                _transfromEffect.RotateCardWhenMoving(eventData.position, true);
                var worldPosition = UIManager.Instance.CameraUI.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, UIManager.Instance.CameraUI.nearClipPlane));
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
            if (!CanInteract) return;

            OnEndDragAction?.Invoke();
            ResetTransform();
            _view.EnableShadow(false);
            _isDragging = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanInteract) return;

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
            if (!CanInteract) return;

            if (_isDragging) return;

            IsChosen = !IsChosen;
            UpdateChosenState(IsChosen);

        }

        public void UpdateChosenState(bool isChosen)
        {
            OnClick?.Invoke(this);
            var sequence = DOTween.Sequence();

            var y = _rectTransform.anchoredPosition.y;
            y += isChosen ? 40 : -40;

            if (!_rectTransform) return;

            sequence.Append(_rectTransform.DOScale(1.2f, 0.025f))
                .Append(_rectTransform.DOAnchorPosY(y, 0.1f))
                .Append(_rectTransform.DOScale(1f, 0.025f));
        }

        public void SetupOriginTransform(Vector3 pos, Vector3 rot, bool playIdleEffect = true)
        {
            _transfromEffect.SetupOriginTransform(pos, rot, playIdleEffect);
        }

        public void SetCardSlot(CardSlot cardSlot)
        {
            if (cardSlot == null)
            {
                _cardSlot = null;
                return;
            }
            _cardSlot = cardSlot;
            transform.SetParent(cardSlot.transform);
        }

        public void RotateCardWhenMoving(Vector3 pos)
        {
            _transfromEffect.RotateCardWhenMoving(pos, true);
        }

        public CardData GetCardData()
        {
            return _data;
        }

        public CardSlot GetCardSlot()
        {
            return _cardSlot;
        }

        public void DoPunch(float duration = 0f, float strength = 0f)
        {
            gameObject.transform.DOPunchScale(Vector3.one * strength, duration);
        }

        public void DoClear(float duration = 0f)
        {
            var currentPos = gameObject.transform.position;
            var newPos = currentPos;
            newPos.x += 20;

            if (gameObject != null)
            {
                _moveTween = DOTween.Sequence();
                _view.EnableBackCard(true);
                _moveTween.Append(gameObject.transform.DORotate(new Vector3(0, -150, 0), 0.2f).SetEase(Ease.InSine))
                    .Append(gameObject.transform.DOMoveX(newPos.x, duration).SetEase(Ease.OutSine));
            }

        }

        private void OnDestroy()
        {
            _moveTween.Kill();
        }
    }
}
