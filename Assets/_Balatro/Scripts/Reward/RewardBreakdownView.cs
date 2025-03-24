using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balatro
{
    public class RewardBreakdownView : MonoBehaviour
    {
        public RectTransform rewardItemGroup; // 👈 Drag RewardItemGroup vào đây
        [SerializeField] private GameObject rewardItemPrefab;
        [SerializeField] private TextMeshProUGUI _cashOut;
        [SerializeField] private TextMeshProUGUI _stageRewardMoney;
        [SerializeField] private TextMeshProUGUI _goalScore;
        [SerializeField] private Image _goalChip;
        

        void Start()
        {
            StartCoroutine(UpdateGroupHeight());
        }
        
        public void RecalculateHeight()
        {
            StartCoroutine(UpdateGroupHeight());
        }

        IEnumerator UpdateGroupHeight()
        {
            yield return null;

            float totalHeight = 0f;

            for (int i = 0; i < rewardItemGroup.childCount; i++)
            {
                RectTransform child = rewardItemGroup.GetChild(i) as RectTransform;
                if (child != null)
                {
                    totalHeight += child.sizeDelta.y;
                }
            }
            
            Vector2 size = rewardItemGroup.sizeDelta;
            size.y = totalHeight;
            rewardItemGroup.sizeDelta = size;
        }

        public void RenderRewards(List<Reward> rewards)
        {
            foreach (Transform child in rewardItemGroup)
            {
                Destroy(child.gameObject);
            }
            
            foreach (Reward reward in rewards)
            {
                GameObject item = Instantiate(rewardItemPrefab, rewardItemGroup);
                RewardItem rewardItem = item.GetComponent<RewardItem>();
                rewardItem.Setup(reward);
            }
        }

        public void RenderStageRewards(StageReward stageReward)
        {
            _stageRewardMoney.text = new string('$', stageReward.Value);
            _goalScore.text = "<sprite index=0> " + stageReward.Score;
            // _goalChip
        }
        
        public void RenderCashOut(int cashOut)
        {
            _cashOut.text = $"Cash Out:  ${cashOut}";
        }
    }
}
