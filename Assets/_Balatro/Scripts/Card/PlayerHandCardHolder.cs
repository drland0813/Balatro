using System;
using System.Collections;
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
        [SerializeField] private Transform _tempCardHolder;

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
            if (!targetCard.CanInteract) return;
            
            if (targetCard.IsChosen)
            {
                if (_chosenCards.Contains(targetCard) || _chosenCards.Count == 5) return;
                _chosenCards.Add(targetCard);
            }
            else
            {
                if (!_chosenCards.Contains(targetCard)) return;
                _chosenCards.Remove(targetCard);
            }
            var interact = _chosenCards.Count < 5;
            {
                foreach (var card in _cards)
                {
                    if (_chosenCards.Contains(card)) continue;

                    card.CanInteract = interact;
                }
            }

            OnCardClicked?.Invoke();
        }

        public List<Card> GetCardsAreChosen()
        {
            Debug.Log($"cards: {_cards.Count} -- choosenCard: {_chosenCards.Count}");
            return _chosenCards;
        }
        
        public void ClearChosenCards(){
            foreach (var card in _cards.ToList())
            {
                if (!_chosenCards.Contains(card))
                {
                    card.CanInteract = true;
                }
                else
                {
                    _cards.Remove(card);
                    card.transform.SetParent(_tempCardHolder);
                }
            }
            Refresh();
        }

        public void DisCard()
        {
            ClearChosenCards();
            StartCoroutine(DiscardCoroutine());
        }

        private IEnumerator DiscardCoroutine()
        {
            var disappearDuration = 0.3f;
            foreach (var card in _chosenCards)
            {
                card.DoClear(disappearDuration);
                disappearDuration -= 0.05f;
                yield return new WaitForSeconds(0.1f);
            }
            AddNewCards();
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