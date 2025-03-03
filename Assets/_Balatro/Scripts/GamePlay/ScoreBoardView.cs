using Balatro;
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


        public void UpdateScore(int score)
        {
            _scoreText.text += score.ToString();
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
    }
}