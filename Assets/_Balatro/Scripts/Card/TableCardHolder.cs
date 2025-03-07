using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balatro
{
    public enum CardEffectPhase
    {
        SetupTransform,
        Pick,
        PlayEffect,
        UnPick,
        Clear
    }
    public class TableCardHolder : HorizontalCardHolder
    {
        public Action OnPlayEffectStart;
        public Action OnPlayEffectFinish;
        public Action<int, Vector3> OnExecuteCard;


        private float _delayMoveCardTime = 0.5f;
        private readonly int _totalCardEffectPhase = 5;
        public void GetCardsData(List<Card> playerHandCards)
        {
            _cards = new List<Card>(playerHandCards);
        }
        public void GenerateCards()
        {
            GenerateSlotCard(_cards.Count);
            StartCoroutine(PlayHandEffectCoroutine());
        }
        
        IEnumerator PlayHandEffectCoroutine()
        {
            var cardCount = _cards.Count;
            var cardValid = _cards;
            var disappearDuration = 0.3f;
            for (var i = 0; i < _totalCardEffectPhase * cardCount; i++)
            {
                var phase = (CardEffectPhase) (i / cardCount);
                var index = i % cardCount;
                switch (phase)
                {
                    case CardEffectPhase.SetupTransform:
                    {
                        var card = _cards[index];
                        var slot = _slots[index];
                        card.SetCardSlot(slot);
                        card.SetupOriginTransform(slot.CardPosition, slot.CardLocalRotation);
                        if (index == cardValid.Count - 1)
                            OnPlayEffectStart?.Invoke();
                        
                        yield return null;

                        break;
                    }
                    case CardEffectPhase.Pick:
                    {
                        if (index == 0)
                            yield return new WaitForSeconds(0.5f);

                        var card = _cards[index];
                        if (!cardValid.Contains(card)) continue;
                        
                        card.UpdateChosenState(true);
                        yield return new WaitForSeconds(0.2f);
                        break;
                    }
                    case CardEffectPhase.PlayEffect:
                    {
                        if (index == 0)
                            yield return new WaitForSeconds(0.5f);

                        var card = _cards[index];
                        if (!cardValid.Contains(card)) continue;

                        OnExecuteCard(card.GetCardData().Chip, card.transform.position);
                        card.DoPunch(0.2f, 0.3f);
                        yield return new WaitForSeconds(0.5f);
                        break;
                    }
                    case CardEffectPhase.UnPick:
                    {
                        if (index == 0)
                            yield return new WaitForSeconds(0.5f);

                        var card = _cards[index];
                        if (!cardValid.Contains(card)) continue;

                        card.UpdateChosenState(false);
                        yield return new WaitForSeconds(0.2f);
                        break;
                    }
                    case CardEffectPhase.Clear:
                    {
                        if (index == 0)
                            yield return new WaitForSeconds(0.3f);

                        var card = _cards[index];
                        // card.SetCardSlot(null);
                        card.DoClear(disappearDuration);
                        disappearDuration -= 0.05f;
                        yield return new WaitForSeconds(0.1f);
                        break;
                    }
                }
            }

            _cards = new List<Card>();
            Refresh(false);
            OnPlayEffectFinish?.Invoke();
        }
    }
}