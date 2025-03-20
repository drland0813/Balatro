using System;
using System.Collections.Generic;
using System.Linq;
using Balatro;
using DG.Tweening;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay
{
    public interface IGameLogic
    {
        public List<Card> SortCardByRank(List<Card> cards);

        public List<Card> SortCardBySuit(List<Card> cards);
    }
    public class GamePlayController : MonoBehaviour, IGameLogic
    {
        [SerializeField] private GamePlayView _gamePlayView;
        
        [SerializeField] private PlayerHandCardHolder _playerHandCard;
        [SerializeField] private HorizontalCardHolder _jokerCard;
        [SerializeField] private HorizontalCardHolder _specialCard;
        [SerializeField] private TableCardHolder _tableCardHolder;

        [SerializeField] private ScoreBoardController _scoreBoardController;
        [SerializeField] private RewardBreakdownController _rewardBreakdownController;

        private int _playHandTurnRemain = 4;
        public int PlayHandTurnRemain
        {
            get => _playHandTurnRemain;
            set
            {
                _playHandTurnRemain = value;
                _gamePlayView.UpdatePlayHandsTurn(value);
            }
        }

        public int DiscardTurnRemain
        {
            get => _discardTurnRemain;
            set
            {
                _discardTurnRemain = value;
                _gamePlayView.UpdatePDiscardTurn(value);
            }
        }

        private int _discardTurnRemain = 4;

        public Action OnWin;
        public Action OnLose;
        

        public void BackToMainMenu()
        {
            OnLose?.Invoke();
        }

        public void Init()
        {
            _tableCardHolder.OnPlayEffectStart = () =>
            {
                _playerHandCard.Refresh();
            };
            _tableCardHolder.OnPlayEffectFinish += _scoreBoardController.UpdateScore;
            _tableCardHolder.OnPlayEffectFinish += () =>
            {
                _scoreBoardController.SetCurrentPokerHand(null);
                _playerHandCard.AddNewCards();
                _playerHandCard.MoveY(moveDown: false);
            };

            _tableCardHolder.OnExecuteCard += _scoreBoardController.ShowChipScore;
            _playerHandCard.OnCardClicked += CheckPokerHandsInfo;

            _playerHandCard.Init();
            _jokerCard.Init();
            _specialCard.Init();
        }

        private void CalculateScore()
        {

        }

        public void SortCardByRank()
        {
            var cards = _playerHandCard.Cards;
            var cardsSorted = SortCardByRank(cards);
            _playerHandCard.Sort(cardsSorted);
            UpdateCardPositionsAfterSort(cardsSorted);
        }

        public void SortCardBySuit()
        {
            var cards = _playerHandCard.Cards;
            var cardsSorted = SortCardBySuit(cards);
            _playerHandCard.Sort(cardsSorted);
            UpdateCardPositionsAfterSort(cardsSorted);
        }

        public List<Card> SortCardByRank(List<Card> cards)
        {
            var sortedCards = cards.OrderBy(card => card.GetCardData().Value).ThenBy(card => (CardType)card.GetCardData().Type).ToList();

            List<CardSlot> originalSlots = GetListCardSlotByPositionX(cards);

            for (int i = 0; i < sortedCards.Count; i++)
            {
                var originalSlot = originalSlots[i];

                sortedCards[i].SetCardSlot(originalSlot);
            }

            return sortedCards;
        }

        public void UpdateCardPositionsAfterSort(List<Card> sortedCards)
        {
            for (int i = 0; i < sortedCards.Count; i++)
            {
                var card = sortedCards[i];
                var newPos = card.GetCardSlot().CardPosition;
                card.SetupOriginTransform(newPos, card.GetCardSlot().CardLocalRotation);
            }
        }


        public List<Card> SortCardBySuit(List<Card> cards)
        {
            var sortedCards = cards.OrderBy(card => GetCardTypePriority((CardType)card.GetCardData().Type)).ThenBy(card => card.GetCardData().Value).ToList();


            List<CardSlot> originalSlots = GetListCardSlotByPositionX(cards);

            for (int i = 0; i < sortedCards.Count; i++)
            {
                var originalSlot = originalSlots[i];

                sortedCards[i].SetCardSlot(originalSlot);
            }

            return cards;
        }

        private int GetCardTypePriority(CardType type)
        {
            switch (type)
            {
                case CardType.Spades:
                    return 0;
                case CardType.Hearts:
                    return 1;
                case CardType.Clubs:
                    return 2;
                case CardType.Diamonds:
                    return 3;
                default:
                    return 0;
            }
        }

        public List<Card> SortCardsByPositionX(List<Card> cards)
        {
            var sortedCards = cards.OrderBy(card => card.transform.position.x).ToList();

            return sortedCards;
        }

        public List<CardSlot> GetListCardSlotByPositionX(List<Card> cards)
        {
            var cardsByPositionX = SortCardsByPositionX(cards);
            List<CardSlot> originalSlots = new List<CardSlot>();
            foreach (var card in cardsByPositionX)
            {
                originalSlots.Add(card.GetCardSlot());
            }
            return originalSlots;
        }

        public void PlayHand()
        {
            if (PlayHandTurnRemain > 0)
            {
                PlayHandTurnRemain--;
                var playerHandCards = _playerHandCard.GetCardsAreChosen();
                if (playerHandCards.Count > 5) return;

                _playerHandCard.ClearChosenCards();
                var type = PokerHandChecker.CheckHand(playerHandCards);
                UpdatePokerHandsInfo(type);
                _tableCardHolder.GetCardsData(playerHandCards);
                _tableCardHolder.SetCardsOnPokerHands(PokerHandChecker.GetHandCards(playerHandCards, type));


                foreach (var card in playerHandCards)
                {
                    card.CanInteract = false;
                    card.IsChosen = false;
                }

                _playerHandCard.MoveY(callback: () =>
                {
                    _tableCardHolder.GenerateCards();
                });
            }
            else
            {
                EndGame();
            }

        }

        private void EndGame()
        {
        }

        public void Discard()
        {
            if (DiscardTurnRemain == 0) return;

            DiscardTurnRemain--;
            _playerHandCard.DisCard();
        }

        private void CheckPokerHandsInfo()
        {
            if (_tableCardHolder.Cards == null || _tableCardHolder.Cards.Count == 0)
            {
                UpdatePokerHandsInfo(_playerHandCard.GetCardsAreChosen());
            }
        }

        private void UpdatePokerHandsInfo(List<Card> chosenCards)
        {
            var type = PokerHandChecker.CheckHand(chosenCards);
            var handData = PokerHandController.Instance.GetHandById((int)type);
            _scoreBoardController.SetCurrentPokerHand(handData);
        }

        private void UpdatePokerHandsInfo(PokerHandType type)
        {
            var handData = PokerHandController.Instance.GetHandById((int)type);
            _scoreBoardController.SetCurrentPokerHand(handData);
        }

        public void EnableRewardBreakdownUI()
        {
            //Fake data
            List<Reward> rewards = new List<Reward>()
            {
                new Reward(
                    RewardType.RemainingHands,
                    null,                       
                    "4",                        
                    "Remaining Hands ($1 each)", 
                    4
                )
            };
            
            _rewardBreakdownController.SetStageReward(new StageReward(null, "300", 3));
            _rewardBreakdownController.SetRewards(rewards);
            
            _gamePlayView.EnableRewardBreakdownUI();
        }

        public void DisableRewardBreakdownUI()
        {
            _gamePlayView.DisableRewardBreakdownUI();
        }
    }
}
