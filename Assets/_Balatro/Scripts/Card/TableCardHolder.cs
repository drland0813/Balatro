using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balatro
{
    public class TableCardHolder : HorizontalCardHolder
    {
        public Action OnPlayEffectFinish;

        private float _delayMoveCardTime = 0.5f;
        
        public void GetCardsData(List<Card> playerHandCards)
        {
            _cards = new List<Card>(playerHandCards);
        }
        public void GenerateCards()
        {
            GenerateSlotCard(_cards.Count);
            StartCoroutine(GetCardsCoroutine());
        }

        IEnumerator GetCardsCoroutine()
        {
            for (var i = 0; i < _cards.Count; i++)
            {
                var slot = _slots[i];
                var newPos = slot.CardPosition;

                var card = _cards[i];
                
                card.SetCardSlot(slot);
                card.SetupOriginTransform(newPos, slot.CardLocalRotation);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            for (var i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                card.UpdateChosenState(true);
                yield return new WaitForSeconds(0.2f);
            }
            OnPlayEffectFinish?.Invoke();
        }
    }
}