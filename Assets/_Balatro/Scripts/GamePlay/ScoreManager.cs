using Balatro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GamePlay
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private ScoreBoardView _view;

        private PokerHand _currentPokerHand;

        private void Start()
        {
            // Temporarily initialize PokerHandManager here
            PokerHandManager.GetInstance().StartNewGame();

            UpdatePokerHandsInformation();
        }

        private int _score
        {
            set => _view.UpdateScore(value);
        }

        public void UpdateScore()
        {
            _score = CalculateScore();
        }

        public void SetCurrentPokerHand(PokerHand pokerHand)
        {
            _currentPokerHand = pokerHand;
            UpdatePokerHandsInformation();
        }

        private void UpdatePokerHandsInformation()
        {
            _view.UpdatePokerHandsInformation(_currentPokerHand);
        }

        private int CalculateScore()
        {
            if (_currentPokerHand == null)
                return 0;

            return _currentPokerHand.point * _currentPokerHand.multiplier;
        }

    }
}