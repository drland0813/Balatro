using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balatro
{
    public class TableCardHolder : HorizontalCardHolder
    {
        public Action OnGetCardComplete;
        private float _delayMoveCardTime = 0.5f;
        public void GetCards(List<Card> playerHandCards)
        {
            StartCoroutine(GetCardsCoroutine(playerHandCards));
            Debug.Log($"card chosen: {playerHandCards.Count}");
        }

        IEnumerator GetCardsCoroutine(List<Card> playerHandCards)
        {
            _cards = new List<Card>();
            for (var i = 0; i < playerHandCards.Count; i++)
            {
                var slot = _slots[i];
                var newPos = slot.CardPosition;

                var card = playerHandCards[i];
                _cards.Add(card);
                
                card.SetCardSlot(slot);
                card.SetupOriginTransform(newPos, slot.CardLocalRotation);
                // yield return new WaitForSeconds(_delayMoveCardTime);
                yield return null;
            }
            OnGetCardComplete?.Invoke();
        }
    }
}