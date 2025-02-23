using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Balatro
{   
    public class DeckManager : MonoBehaviour
    {
        
    }

    [Serializable]
    public class CardConfig
    {
        public List<CardData> Cards;
    }

    public class HorizontalCardHolder : MonoBehaviour
    {
        [SerializeField] protected CardSlot _cardSlotPrefab; 
        [SerializeField] protected Card _cardPrefab; 
        [SerializeField] protected Transform _cardParent;
        [SerializeField] protected CardResourceDataAsset _cardAsset;
        [SerializeField] protected int _numberOfCard;

        [Header("Alignment Cards")] 
        [SerializeField] protected bool _isAlignment;
        [SerializeField] protected float radius = 30;   // Bán kính đường cong
        [SerializeField] protected float maxRotationAngle = 10; // Góc xoay tối đa
        [SerializeField] protected float offsetX = 0f;    // Điểm bắt đầu trục X
        [SerializeField] protected float offsetY = -100f; 
        
        
        protected Card _selectedCard;
        protected List<CardData> _cardData;
        protected List<Card> _cards;
        protected bool _isCrossing;
        protected List<Card> _chosenCards;
        protected List<CardSlot> _slots;

        public List<Card> Cards => _cards;
        public List<Card> ChosenCards => _chosenCards;

        public void Init()
        {
            GetCardData();
            GenerateSlotCard(_numberOfCard);
            GenerateCards();
        }

        private void GenerateCards()
        {
            _chosenCards = new List<Card>();
            _cards = new List<Card>();
            var randomCards = new List<CardData>();
    
            var random = new System.Random();

            var tempCards = new List<CardData>(_cardData);

            for (var i = 0; i < _numberOfCard && tempCards.Count > 0; i++)
            {
                var randomIndex = random.Next(0, tempCards.Count);
                randomCards.Add(tempCards[randomIndex]);
                tempCards.RemoveAt(randomIndex);
            }

            for (var i = 0; i < _slots.Count; i++)
            {
                var cardSlot = _slots[i];
                var cardData = randomCards[i];
                var card = CreateCard(cardData, cardSlot);
                card.OnBeginDragAction = BeginDragCard;
                card.OnDragAction = DraggingCard;
                card.OnEndDragAction = EndDragCard;
                card.OnClick = ClickOnCard;

                _cards.Add(card);
            }
        }

        public virtual void ClickOnCard(Card targetCard){}

        protected void BeginDragCard(Card card)
        {
            _selectedCard = card;
        }

        private void DraggingCard()
        {
            if (_selectedCard == null) return;

            if (_isCrossing) return;
            
            foreach (var card in _cards)
            {
                if (card == _selectedCard) continue;

                if (_selectedCard.transform.position.x > card.transform.position.x)
                {
                    var selectedParentIndex = _selectedCard.transform.parent.GetSiblingIndex();
                    var cardParentIndex = card.transform.parent.GetSiblingIndex();
                    if (selectedParentIndex < cardParentIndex)
                    {
                        SwapCard(card);
                        break;
                    }
                }
                
                if (_selectedCard.transform.position.x < card.transform.position.x)
                {
                    var selectedParentIndex = _selectedCard.transform.parent.GetSiblingIndex();
                    var cardParentIndex = card.transform.parent.GetSiblingIndex();
                    if (selectedParentIndex > cardParentIndex)
                    {
                        SwapCard(card);
                        break;
                    }
                }
            }
        }

        protected void SwapCard(Card card)
        {
            _isCrossing = true;
            var selectedCardSlot = _selectedCard.transform.parent.GetComponent<CardSlot>();
            var cardSlotToSwap = card.transform.parent.GetComponent<CardSlot>();
            card.SetCardSlot(selectedCardSlot);
            _selectedCard.SetCardSlot(cardSlotToSwap);
            _isCrossing = false;
            
            var newPos = selectedCardSlot.CardPosition;
            if (card.IsChosen)
            {
                newPos.y += 40;
            }
            card.SetupOriginTransform(newPos, selectedCardSlot.CardLocalRotation);
        }
        
        protected void SwapCard(Card cardA, Card cardB)
        {
            _isCrossing = true;
            var cardASlot = cardA.transform.parent.GetComponent<CardSlot>();
            var cardBSlot = cardB.transform.parent.GetComponent<CardSlot>();
            cardA.SetCardSlot(cardBSlot);
            cardB.SetCardSlot(cardASlot);
            _isCrossing = false;
            
            cardA.SetupOriginTransform(cardBSlot.CardPosition, cardBSlot.CardLocalRotation);
            cardB.SetupOriginTransform(cardASlot.CardPosition, cardASlot.CardLocalRotation);

        }

        protected void EndDragCard()
        {
            _selectedCard = null;
        }
        
        


        protected void GetCardData()
        {
            _cardData = new List<CardData>();
            var cardData = Resources.Load<TextAsset>("Config/Card/NormalCard");
            if (cardData != null)
            {
                _cardData = JsonUtility.FromJson<CardConfig>(cardData.text).Cards;
            }
        }

        protected void GenerateSlotCard(int numberOfCard)
        {
            _slots = new List<CardSlot>();
            for (int i = 0; i < numberOfCard; i++)
            {
                var cardSlot = Instantiate(_cardSlotPrefab, _cardParent);
                cardSlot.name = $"Slot-{i}";
                _slots.Add(cardSlot);

                if (_isAlignment)
                {
                    InitCardTransformValue(cardSlot, i);
                }
            }
        }

        protected Card CreateCard(CardData data, CardSlot slot)
        {
            var card = Instantiate(_cardPrefab, slot.transform);
            var sprite = _cardAsset.GetCardSprite(data.Type, data.ID);
            card.Init(data);
            card.SetupView(sprite);
            card.SetupOriginTransform(slot.CardPosition, slot.CardLocalRotation);
            return card;
        }
        
        protected void InitCardTransformValue(CardSlot slot, int index)
        {
            var mid = _numberOfCard / 2;
            var value = mid - index;
            float y = 10 * (index >= mid ? (_numberOfCard - (index + 1)) : index);
            float rotationZ = value;

            slot.SetCardTransform(new Vector2(0, y),new Vector3(0f, 0f, rotationZ));
        }

        public void Refresh()
        {
            var currentCards = _cards;
            // foreach (var card in currentCards)
            // {
            //     card.transform.SetParent(transform);
            //     var cardSlot = card.GetCardSlot();
            //     // card.SetCardSlot(null);
            // }
            foreach (var slot in _slots)
            {
                if (slot.transform.childCount == 0)
                {
                    Destroy(slot.gameObject);
                }
            }
            
            // GenerateSlotCard(currentCards.Count);
            //
            //
            // for (int i = 0; i < currentCards.Count; i++)
            // {
            //     var slot = _slots[i];
            //     var newPos = slot.CardPosition;
            //
            //     var card = _cards[i];
            //     
            //     card.SetCardSlot(slot);
            //     card.SetupOriginTransform(newPos, slot.CardLocalRotation);
            // }
            
        }

        public void Sort(List<Card> cardsSorted)
        {
            
            _cards = cardsSorted;
        }
        
        protected void DoMoveY(float yValue, Action callback = null, bool isDown = true)
        {
            var pos = transform.position;
            pos.y += isDown ? -yValue : yValue;
            transform.DOMoveY(pos.y, 0.5f).SetEase(Ease.InBack)
                .OnComplete((() =>
                {
                    callback?.Invoke(); 
                }));
        }
    }
}
