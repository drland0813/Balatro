using System;
using Balatro;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay
{
    public class ScoreBoardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        [SerializeField] private TextMeshProUGUI _headerName;

        [SerializeField] private Image _goalChip;
        [SerializeField] private TextMeshProUGUI _goalScore;
        [SerializeField] private TextMeshProUGUI _rewardMoney;

        [SerializeField] private TextMeshProUGUI _pokerHandsName;
        [SerializeField] private TextMeshProUGUI _pokerHandsLevel;
        [SerializeField] private TextMeshProUGUI _pokerHandsPoint;
        [SerializeField] private TextMeshProUGUI _pokerHandsMultiplier;

        [SerializeField] private TextMeshProUGUI _handsNumer;
        [SerializeField] private TextMeshProUGUI _discardsNumer;
        [SerializeField] private TextMeshProUGUI _Money;
        [SerializeField] private TextMeshProUGUI _AnteNumber;
        [SerializeField] private TextMeshProUGUI _RoundNumber;

        [SerializeField] private ChipScoreUI _chipScoreUI;
        private string _scoreIconText;

        private void Start()
        {
            _scoreIconText = _scoreText.text;
            UpdateScore(0);
        }

        public void UpdateScore(int score)
        {
            _scoreText.text = _scoreIconText + score;
        }

        public void UpdatePokerHandsInformation(PokerHand pokerHand)
        {
            if (pokerHand == null)
            {
                _pokerHandsName.text = "";
                _pokerHandsLevel.text = "";
                _pokerHandsPoint.text = "0";
                _pokerHandsMultiplier.text = "0";
                return;
            }

            _pokerHandsName.text = pokerHand.name;
            _pokerHandsLevel.text = "Lv" + pokerHand.level;
            _pokerHandsPoint.text = pokerHand.point.ToString();
            _pokerHandsMultiplier.text = pokerHand.multiplier.ToString();
        }

        public void UpdatePokerHandsScore(PokerHand pokerHand)
        {
            _pokerHandsPoint.text = pokerHand.point.ToString();
            _pokerHandsMultiplier.text = pokerHand.multiplier.ToString();
        }

        public void ShowChipScore(int chip, Vector3 position)
        {
            var newPos = position;
            newPos.y += 1f;
            var newYValue = newPos.y + 1f;

            _chipScoreUI.transform.position = newPos;
            _chipScoreUI.Init(chip);
            _chipScoreUI.transform.DOMoveY(newYValue, 0.5f).OnComplete(() =>
            {
                _chipScoreUI.Show(false);
            });
        }
    }
}