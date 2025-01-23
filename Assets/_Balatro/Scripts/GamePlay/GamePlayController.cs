using System;
using System.Collections.Generic;
using Balatro;
using UnityEngine;
namespace GamePlay
{
    public interface IGameLogic
    {
        public List<Card> SortCardByRank(List<Card> cards);

        public List<Card> SortCardBySuit(List<Card> cards);
    }
    public class GamePlayController : MonoBehaviour, IGameLogic
    {
        [SerializeField] private HorizontalCardHolder _playerHandCard;
        [SerializeField] private HorizontalCardHolder _jokerCard;
        [SerializeField] private HorizontalCardHolder _specialCard;

        public Action OnWin;
        public Action OnLose;
        
        public void BackToMainMenu()
        {
            OnLose?.Invoke();
        }

        public void Init()
        {
            _playerHandCard.Init();
            _jokerCard.Init();
            _specialCard.Init();
        }

        public void SortCardByRank()
        {
            var cards = _playerHandCard.Cards;
            var cardsSorted = SortCardByRank(cards);
            _playerHandCard.Sort(cardsSorted);
        }
        
        public void SortCardBySuit()
        {
            var cards = _playerHandCard.Cards;
            var cardsSorted = SortCardBySuit(cards);
            _playerHandCard.Sort(cardsSorted);
        }

        public List<Card> SortCardByRank(List<Card> cards)
        {
            
            
            return cards;
        }

        public List<Card> SortCardBySuit(List<Card> cards)
        {
            
            
            return cards;
        }
    }
}
