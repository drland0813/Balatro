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
        [SerializeField] private CardSlot _cardSlotPrefab; 
        [SerializeField] private Card _cardPrefab; 
        [SerializeField] private Transform _cardParent;
        [SerializeField] private CardResourceDataAsset _cardAsset;
        [SerializeField] private int _numberOfCard;

        [Header("Alignment Cards")] 
        [SerializeField] private bool _isAlignment;
        [SerializeField] private float radius = 30;   // Bán kính đường cong
        [SerializeField] private float maxRotationAngle = 10; // Góc xoay tối đa
        [SerializeField] private float offsetX = 0f;    // Điểm bắt đầu trục X
        [SerializeField] private float offsetY = -100f; 
        
        
        private Card _selectedCard;
        private List<CardData> _cardData;
        private List<Card> _cards;
        private bool _isCrossing;


        private void Start()
        {
            GetCardData();
            GeneratePlayerHandCards();
        }

        private void GeneratePlayerHandCards()
        {
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

            for (var i = 0; i < randomCards.Count; i++)
            {
                var cardData = randomCards[i];
                var card = CreateCard(cardData, i);
                card.OnBeginDragAction = BeginDragCard;
                card.OnDragAction = DraggingCard;
                card.OnEndDragAction = EndDragCard;

                _cards.Add(card);
            }
        }

        private void BeginDragCard(Card card)
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

        private void SwapCard(Card card)
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

        private void EndDragCard()
        {
            _selectedCard = null;
        }
        
        


        private void GetCardData()
        {
            _cardData = new List<CardData>();
            var cardData = Resources.Load<TextAsset>("Config/Card/NormalCard");
            if (cardData != null)
            {
                _cardData = JsonUtility.FromJson<CardConfig>(cardData.text).Cards;
            }
        }

        private Card CreateCard(CardData data, int i)
        {
            var cardSlot = Instantiate(_cardSlotPrefab, _cardParent);
            cardSlot.name = $"Slot-{i}";
            if (_isAlignment)
            {
                InitCardTransformValue(cardSlot, i);
            }
            var card = Instantiate(_cardPrefab, cardSlot.transform);
            var sprite = _cardAsset.GetCardSprite(data.Type, data.ID);
            card.Init(data);
            card.SetupView(sprite);
            card.SetupOriginTransform(cardSlot.CardPosition, cardSlot.CardLocalRotation);
            return card;
        }
        
        private void InitCardTransformValue(CardSlot slot, int index)
        {
            var mid = _numberOfCard / 2;
            var value = mid - index;
            float y = 10 * (index >= mid ? (_numberOfCard - (index + 1)) : index);
            float rotationZ = value;

            slot.SetCardTransform(new Vector2(0, y),new Vector3(0f, 0f, rotationZ));
        }
    }
}
