using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace Balatro
{
    public class PlayerHandCardHolder : HorizontalCardHolder
    {
        [SerializeField] private GameObject _buttonGroup;
        [SerializeField] private PokerHandType _pokerHandType;

        public event Action OnCardClicked;

        public override void Init()
        {
            GetCardData();
            GenerateSlotCard(_numberOfCard);
            if (_pokerHandType == PokerHandType.None)
                GenerateCards(_numberOfCard);
            else
            {
                GenerateCardsByPokerType(_pokerHandType);
            }
        }

        public override void ClickOnCard(Card targetCard)
        {
            if (targetCard.IsChosen)
            {
                if (_chosenCards.Contains(targetCard)) return;
                _chosenCards.Add(targetCard);
            }
            else
            {
                if (!_chosenCards.Contains(targetCard)) return;
                _chosenCards.Remove(targetCard);
            }

            OnCardClicked?.Invoke();
        }

        public List<Card> GetCardsAreChosen()
        {
            foreach (var card in _chosenCards)
            {
                _cards.Remove(card);
            }
            return _chosenCards;
            
        }

        public void DisCard()
        {
            foreach (var card in _chosenCards.ToList())
            {
                // card.UpdateChosenState();
                _chosenCards.Remove(card);
            }
        }

        public void MoveY(Action callback = null ,bool moveDown = true)
        {
            DoMoveY(1.7f, callback, moveDown);
            _buttonGroup.SetActive(!moveDown);
        }

        [Button("Re-Gen")]
        public void ReGenerateCardByType()
        {
            Refresh(false);
            GenerateSlotCard(_numberOfCard);
            GenerateCardsByPokerType(_pokerHandType);
        }
    }
}