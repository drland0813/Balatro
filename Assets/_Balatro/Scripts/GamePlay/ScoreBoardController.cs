using System;
using System.Collections;
using Balatro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GamePlay
{
    public class ScoreBoardController : MonoBehaviour
    {
        [SerializeField] private ScoreBoardView _view;
        [SerializeField] private GamePlayView _gamePlayView;
        
        public GamePlayController gamePlayController;

        private PokerHand _currentPokerHand;
        private int _score = 0;

        private void Start()
        {
            // Temporarily initialize PokerHandManager here
            PokerHandController.GetInstance().StartNewGame();

            UpdatePokerHandsInformation();
        }

        public void UpdateScore()
        {
            _score = _score + GetCurrentHandsScore();
            _view.UpdateScore(_score);
    
            // Gọi coroutine để delay
            StartCoroutine(DelayCheckStageRequirement(_score));
        }

        private IEnumerator DelayCheckStageRequirement(int score)
        {
            yield return new WaitForSeconds(1f);
            HasReachedStageScoreRequirement(score);
        }

        private void HasReachedStageScoreRequirement(int score)
        {
            if (score > 300)
            {
                gamePlayController.EnableRewardBreakdownUI();
            }
        }

        public void SetCurrentPokerHand(PokerHand pokerHand)
        {
            _currentPokerHand = pokerHand;
            UpdatePokerHandsInformation();
        }

        public PokerHand GetCurrentPokerHand()
        {
            return _currentPokerHand;
        }

        private void UpdatePokerHandsInformation()
        {
            _view.UpdatePokerHandsInformation(_currentPokerHand);
        }

        private void UpdatePokerHandsScore()
        {
            _view.UpdatePokerHandsScore(_currentPokerHand);
        }

        private int GetCurrentHandsScore()
        {
            if (_currentPokerHand == null)
                return 0;

            return _currentPokerHand.point * _currentPokerHand.multiplier;
        }

        public void ShowChipScore(int chip, Vector3 position)
        {
            _currentPokerHand.point += chip;
            _view.ShowChipScore(chip, position);
            UpdatePokerHandsScore();
        }
    }
}