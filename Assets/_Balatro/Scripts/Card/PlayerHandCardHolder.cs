using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Balatro
{
    public class PlayerHandCardHolder : HorizontalCardHolder
    {
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
                card.UpdateChosenState();
                _chosenCards.Remove(card);
            }
        }
    }
}