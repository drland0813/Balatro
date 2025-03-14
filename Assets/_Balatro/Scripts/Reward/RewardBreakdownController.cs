using System.Collections;
using System.Collections.Generic;
using GamePlay;
using UnityEngine;

namespace Balatro
{
    public class RewardBreakdownController : MonoBehaviour
    {
        [SerializeField] private RewardBreakdownView view;
        public GamePlayController gamePlayController;

        private StageReward _stageReward;
        private List<Reward> _rewards = new List<Reward>();
        private int _cashOut;

        public void SetStageReward(StageReward stageReward)
        {
            _stageReward = stageReward;
            RenderStageRewards();
            CalculateCashOut(_stageReward);
        }
        
        public void SetRewards(List<Reward> rewardList)
        {
            _rewards = rewardList;
            RenderRewards();
            CalculateCashOut(rewardList);
        }

        private void RenderRewards()
        {
            view.RenderRewards(_rewards);
        }

        private void RenderStageRewards()
        {
            view.RenderStageRewards(_stageReward);
        }

        private void CalculateCashOut(StageReward stageReward)
        {
            _cashOut = stageReward.Value;
            view.RenderCashOut(_cashOut);
        }
        
        private void CalculateCashOut(List<Reward> rewardList)
        {
            int totalValue = 0;
            foreach (var reward in rewardList)
            {
                totalValue += reward.Value;
            }
            _cashOut += totalValue;
            view.RenderCashOut(_cashOut);
        }

        public void CashOutMoney()
        {
            MoneyController.GetInstance().AddMoney(_cashOut);
            gamePlayController.DisableRewardBreakdownUI();
        }
    }
}
