using Common.UI;
using TMPro;
using UnityEngine;

namespace GamePlay
{
    public class GamePlayView : UIController
    {
        [SerializeField] private TextMeshProUGUI _playHandsTurnText;
        [SerializeField] private TextMeshProUGUI _discardTurnText;

        public GameObject _runInfoView;
        public GameObject _rewardBreakdownView;

        public void EnableRunInfoUI()
        {
            _runInfoView.SetActive(true);
        }

        public void DisableRunInfoUI()
        {
            _runInfoView.SetActive(false);
        }
        
        public void EnableRewardBreakdownUI()
        {
            _rewardBreakdownView.SetActive(true);
        }

        public void DisableRewardBreakdownUI()
        {
            _rewardBreakdownView.SetActive(false);
        }

        public void UpdatePlayHandsTurn(int value)
        {
            _playHandsTurnText.text = value.ToString();
        }
        
        public void UpdatePDiscardTurn(int value)
        {
            _discardTurnText.text = value.ToString();
        }
    }
}