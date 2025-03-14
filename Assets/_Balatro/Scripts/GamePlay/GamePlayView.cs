using Common.UI;
using UnityEngine;

namespace GamePlay
{
    public class GamePlayView : UIController
    {

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
    }
}