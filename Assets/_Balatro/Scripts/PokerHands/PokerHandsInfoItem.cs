using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Balatro
{
    public class PokerHandsInfoItem : MonoBehaviour
    {
        public TextMeshProUGUI _namePokerHandText;
        public TextMeshProUGUI _pointText;
        public TextMeshProUGUI _multiplierText;
        public TextMeshProUGUI _levelText;
        public TextMeshProUGUI _numberOfTimesText;

        public void SetPokerHandData(PokerHand hand)
        {
            _namePokerHandText.text = hand.name;
            _pointText.text = $"{hand.point}";
            _multiplierText.text = $"{hand.multiplier}";
            _levelText.text = $"lvl.{hand.level}";
            _numberOfTimesText.text = $"{hand.numberOfTimes}";
        }
    }
}
